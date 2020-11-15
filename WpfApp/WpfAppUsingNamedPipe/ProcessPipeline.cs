using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfAppUsingNamedPipe
{
    public class ProcessPipelineServer : IDisposable
    {
        private const string PIPE_NAME = "UnityAsALibraryPipelinePCHost";
        private NamedPipeServerStream stream;

        public async Task ConnectionAsync()
        {
            try
            {
                stream = new NamedPipeServerStream(PIPE_NAME);

                await stream.WaitForConnectionAsync();

                Debug.WriteLine("server stream connected.");
            }
            catch (System.IO.IOException)
            {
                Debug.WriteLine("server connection is closed.");
            }
        }
        public async Task Write(byte[] data)
        {
            try
            {
                if (stream == null || !stream.IsConnected)
                {
                    Debug.WriteLine("server stream don't ready.");
                    return;
                }
                await stream.WriteAsync(data);
                stream.WaitForPipeDrain();
            }
            catch (System.IO.IOException)
            {
                Debug.WriteLine("server connection is closed.");
            }
        }

        public void Dispose()
        {
            stream.Dispose();
            stream = null;
        }
    }
    public class ProcessPipelineClient : IDisposable
    {
        private NamedPipeClientStream stream;
        private Task senderTask;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public Action<byte[]> OnReceived;

        public async Task ConnectionAsync()
        {
            stream = new NamedPipeClientStream(".", "UnityAsALibraryPipelineUnityHost", PipeDirection.InOut, PipeOptions.Asynchronous);

            await stream.ConnectAsync(tokenSource.Token);

            senderTask = Task.Run(async () =>
            {
                using (var reader = new StreamReader(stream))
                {
                    while (stream.IsConnected)
                    {
                        var buffer = new byte[1048];

                        var res = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (res == 0)
                        {
                            Debug.WriteLine("data is empty.");
                            break;
                        }
                        Debug.WriteLine("Data :" + buffer.Length);

                        OnReceived?.Invoke(buffer);

                        Thread.Sleep(100);

                        if (tokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                }
            },
            tokenSource.Token);
        }
        public void Dispose()
        {
            tokenSource.Cancel();
            senderTask.Dispose();
            senderTask = null;
            stream.Dispose();
            stream = null;
        }
    }
}
