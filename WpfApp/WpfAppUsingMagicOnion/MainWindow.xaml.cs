using Grpc.Core;
using MagicOnion.Client;
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

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UnityHwndHost host;
        private CommunicationHub hub;
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
                host = new UnityHwndHost(@"UnityApp\UnityApp.exe", "-possibly -other -arguments");
                grid.Children.Add(host);
            }

            hub = new CommunicationHub();
            hub.OnMovePositionCallback = (position, rotation, name) =>
            {
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
            hub.OnSendMessageCallback = (name, msg) =>
            {
                textMessage.Text += $"{name} : {msg}\r\n";
            };
            hub.OnJoinCallback = async name =>
            {
                if (name != myName)
                {
                    await hub.SendMessageAsync($"Hello {name}.");
                    await hub.SendMessageAsync($"Hello inside world.");
                }
            };
            await hub.JoinAsync(myName);
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) { return; }

            var axis = (sender == radioX) ? RotatableAxis.X
                : (sender == radioY) ? RotatableAxis.Y
                : RotatableAxis.XY;

            await hub.UpdateRotatableAxis(axis);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isCleanuped) { return; }

            if (!isCalledQuit)
            {
                Quit();
            }
            e.Cancel = true;
        }
        private bool isCalledQuit;
        private bool isCleanuped;
        private async void Quit()
        {
            isCalledQuit = true;

            host.Destroy();
            await hub?.Dispose();

            isCleanuped = true;

            this.Close();
        }
    }
}
