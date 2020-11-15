using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ProcessPipelineServer : MonoBehaviour
{
    private const string PIPE_NAME = "UnityAsALibraryPipelineUnityHost";
    private NamedPipeServerStream stream;
    private CancellationTokenSource tokenSource = new CancellationTokenSource();

    public async UniTask ConnectionAsync()
    {
        try
        {
            stream = new NamedPipeServerStream(PIPE_NAME);

            await UniTask.Run(() => stream.WaitForConnection());

            Debug.Log("server stream connected.");
        }
        catch (System.IO.IOException)
        {
            Debug.Log("server connection is closed.");
        }
    }
    public async Task Write(byte[] data)
    {
        try
        {
            if (stream == null || !stream.IsConnected)
            {
                Debug.Log("server stream don't ready.");
                return;
            }
            await stream.WriteAsync(data, 0, data.Length, tokenSource.Token);
            stream.WaitForPipeDrain();
        }
        catch (System.IO.IOException)
        {
            Debug.Log("server connection is closed.");
        }
    }

    public void Dispose()
    {
        stream.Dispose();
        stream = null;
    }

    void OnDestroy()
    {
        tokenSource.Cancel();
        Debug.Log("task cancelled.");
        Dispose();
    }
}
