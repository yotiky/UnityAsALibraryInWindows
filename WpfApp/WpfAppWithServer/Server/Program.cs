using Grpc.Core;
using Grpc.Core.Logging;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Server
{
    public static class Program
    {
        private static CancellationTokenSource tokenSource;
        private static IHost host;

        public static async Task Run(DelegateLogger dLogger)
        {
            //GrpcEnvironment.SetLogger(new Grpc.Core.Logging.ConsoleLogger());
            GrpcEnvironment.SetLogger(new CompositeLogger(
                new DebugConsoleLogger(),
                dLogger));

            tokenSource = new CancellationTokenSource();

            using var ihost = await MagicOnionHost.CreateDefaultBuilder()
                .UseMagicOnion(
                    new MagicOnionOptions(isReturnExceptionStackTraceInErrorDetail: true),
                    new ServerPort("0.0.0.0", 12345, ServerCredentials.Insecure))
                //.UseConsoleLifetime()
                .StartAsync(tokenSource.Token);
            host = ihost;
            
            await ihost.WaitForShutdownAsync();
        }

        public static async Task Stop()
        {
            //tokenSource.Cancel();
            //Debug.WriteLine("ShutdownChannelsAsync");
            //await GrpcEnvironment.ShutdownChannelsAsync();
            //Debug.WriteLine("KillServersAsync");
            //await GrpcEnvironment.KillServersAsync();

            //await host.StopAsync();
            //tokenSource.Cancel();
            await host.StopAsync();
            host = null;
            Debug.WriteLine("Stopped...");
        }
    }


    public class DebugConsoleLogger : ILogger
    {
        #region ILogger implementations
        public ILogger ForType<T>() => this;
        public void Debug(string message) => this.Log("D", message);
        public void Debug(string format, params object[] formatArgs) => this.Debug(string.Format(format, formatArgs));
        public void Info(string message) => this.Log("I", message);
        public void Info(string format, params object[] formatArgs) => this.Info(string.Format(format, formatArgs));
        public void Warning(string message) => this.Log("W", message);
        public void Warning(string format, params object[] formatArgs) => this.Warning(string.Format(format, formatArgs));
        public void Warning(Exception exception, string message) => this.Warning(message + " " + exception);
        public void Error(string message) => this.Log("E", message);
        public void Error(string format, params object[] formatArgs) => this.Error(string.Format(format, formatArgs));
        public void Error(Exception exception, string message) => this.Error(message + " " + exception);
        #endregion

        private void Log(string severity, string message)
        {
            var text = $"{severity} - {DateTime.Now:yyyy/MM/dd HH:mm:ss.ffffff} | {message}";
            System.Diagnostics.Debug.WriteLine(text);
        }
    }
    public class DelegateLogger : ILogger
    {
        private Action<string> write;
        public DelegateLogger(Action<string> write)
        {
            this.write = write;
        }
        #region ILogger implementations
        public ILogger ForType<T>() => this;
        public void Debug(string message) => this.Log("D", message);
        public void Debug(string format, params object[] formatArgs) => this.Debug(string.Format(format, formatArgs));
        public void Info(string message) => this.Log("I", message);
        public void Info(string format, params object[] formatArgs) => this.Info(string.Format(format, formatArgs));
        public void Warning(string message) => this.Log("W", message);
        public void Warning(string format, params object[] formatArgs) => this.Warning(string.Format(format, formatArgs));
        public void Warning(Exception exception, string message) => this.Warning(message + " " + exception);
        public void Error(string message) => this.Log("E", message);
        public void Error(string format, params object[] formatArgs) => this.Error(string.Format(format, formatArgs));
        public void Error(Exception exception, string message) => this.Error(message + " " + exception);
        #endregion

        private void Log(string severity, string message)
        {
            var text = $"{severity} - {DateTime.Now:yyyy/MM/dd HH:mm:ss.ffffff} | {message}";
            write?.Invoke(text);
        }
    }
}
