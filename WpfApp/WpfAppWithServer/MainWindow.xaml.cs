using Grpc.Core;
using MagicOnion.Client;
using MagicOnion.Hosting;
using MagicOnion.Server;
using Microsoft.Extensions.Hosting;
using Sample.Shared;
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

namespace WpfAppWithServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommunicationHub hub;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            stop.IsEnabled = false;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Server.Program.Run(new Server.DelegateLogger(
                log => { Dispatcher.Invoke(() => textLog.Text += log + "\r\n"); }));
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) { return; }

            var axis = (sender == radioX) ? RotatableAxis.X
                : (sender == radioY) ? RotatableAxis.Y
                : RotatableAxis.XY;

            await hub.UpdateRotatableAxis(axis);
        }

        private async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isCleanuped) { return; }

            if (!isCalledQuit)
            {
                await Quit(true);
            }
            e.Cancel = true;
        }
        private bool isCalledQuit;
        private bool isCleanuped;
        private async Task Quit(bool close)
        {
            isCalledQuit = true;

            if (host != null)
            {
                host.Destroy();
                host = null;
            }
            if (hub != null)
            {
                await hub.Dispose();
                hub = null;
            }

            isCleanuped = true;

            if (close)
            {
                await Server.Program.Stop();
                this.Close();
            }
        }

        UnityHwndHost host;
        private async void start_Click(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                host = new UnityHwndHost(@"UnityApp\UnityApp.exe", "");
                grid.Children.Add(host);
            }

            hub = new CommunicationHub();
            hub.OnMovePositionCallback = (position, rotation, name) =>
            {
                if (name != "Camera") { return; }
                textPosition.Text = $"{name}\r\n" +
                $"Position\r\n" +
                $" x:{position.x}\r\n" +
                $" y:{position.y}\r\n" +
                $" z:{position.z}\r\n" +
                $"Rotation\r\n" +
                $" x:{rotation.x}\r\n" +
                $" y:{rotation.y}\r\n" +
                $" z:{rotation.z}\r\n" +
                $" w:{rotation.w}";
            };
            hub.OnSendMessageCallback = (name, msg)
                => textMessage.Text += $"{name} : {msg}\r\n";

            await hub.JoinAsync("MainWindow");
            await hub.SendMessageAsync("Hello another world.");

            start.IsEnabled = false;
            stop.IsEnabled = true;
        }
        private async void stop_Click(object sender, RoutedEventArgs e)
        {
            if (isCalledQuit) { return; }

            if (!isCalledQuit)
            {
                await Quit(false);
            }

            start.IsEnabled = true;
            stop.IsEnabled = false;
            isCalledQuit = false;
            isCleanuped = false;
        }
    }
}
