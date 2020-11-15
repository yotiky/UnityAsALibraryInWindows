using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
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
}
