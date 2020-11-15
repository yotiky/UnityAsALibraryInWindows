using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ProcessPipelineClient : MonoBehaviour
{
    CancellationTokenSource tokenSource = new CancellationTokenSource();

    private readonly Subject<byte[]> onReceivedSubject = new Subject<byte[]>();
    public IObservable<byte[]> OnReceived => onReceivedSubject;

    void Start()
    {
        this.UpdateAsObservable()
            .First()
            .Subscribe(async _ =>
            {
                await UniTask.Run(async () =>
                {
                    using (var stream = new NamedPipeClientStream(".", "UnityAsALibraryPipelinePCHost", PipeDirection.InOut, PipeOptions.Asynchronous))
                    {
                        var tryConnect = true;
                        while (tryConnect)
                        {
                            try
                            {
                                stream.Connect();
                                tryConnect = false;
                            }
                            catch (Win32Exception)
                            {
                                await UniTask.Delay(500);
                                Debug.LogWarning("retry connect stream.");
                            }
                        }
                        Debug.Log("stream connected.");

                        while (true)
                        {
                            var buffer = new byte[1048];

                            var res = await stream.ReadAsync(buffer, 0, buffer.Length);
                            if (res == 0)
                            {
                                Debug.Log("data is empty.");
                                break;
                            }
                            Debug.Log("Data :" + buffer.Length);

                            onReceivedSubject.OnNext(buffer);

                            await UniTask.DelayFrame(1);

                            if (tokenSource.Token.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    }
                    Debug.Log("End of pipeline.");
                },
                cancellationToken: tokenSource.Token);
            })
            .AddTo(this);
            
    }

    void OnDestroy()
    {
        tokenSource.Cancel();
        Debug.Log("task cancelled.");
    }
}
