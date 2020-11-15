using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnityEngine;

namespace WpfAppUsingNamedPipe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UnityHwndHost host;
        private ProcessPipelineServer pipelineServer;
        private ProcessPipelineClient pipelineClient;
        private const string myName = "MainWindow";

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                host = new UnityHwndHost(@"UnityApp\UnityAppUsingNamedPipe.exe", "-possibly -other -arguments");
                grid.Children.Add(host);
            }

            pipelineServer = new ProcessPipelineServer();
            await pipelineServer.ConnectionAsync();

            var msg = new Letter
            {
                Name = myName,
                Message = "Hello inside world.",
            };
            await SendMessage(msg);

            pipelineClient = new ProcessPipelineClient();
            pipelineClient.OnReceived += body =>
            {
                var deserialized = MessagePackSerializer.Deserialize<IMessageBody>(body);
                if (deserialized is Letter letter)
                {
                    this.Dispatcher.Invoke(() => textMessage.Text += $"{letter.Name} : {letter.Message}\r\n");
                }
            };
            await pipelineClient.ConnectionAsync();
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) { return; }

            var axis = (sender == radioX) ? RotatableAxis.X
                : (sender == radioY) ? RotatableAxis.Y
                : RotatableAxis.XY;

            var msg = new UpdateRotatableAxis { Axis = axis };

            await SendMessage(msg);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isCleanuped) { return; }

            if (!isCalledQuit)
            {
                Quit();
            }
            //e.Cancel = true;
        }
        private bool isCalledQuit;
        private bool isCleanuped;
        private void Quit()
        {
            isCalledQuit = true;

            host.Destroy();
            pipelineServer?.Dispose();

            isCleanuped = true;

            //this.Close();
        }

        private async Task SendMessage(IMessageBody msg)
        {
            var serialized = MessagePackSerializer.Serialize<IMessageBody>(msg);
            await pipelineServer.Write(serialized);
        }
    }
}
