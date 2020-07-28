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

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                grid.Children.Add(new UnityHost
                {
                    AppPath = @"C:\git\yotiky\Sample.UnityAppInWPF\UnityApp\App\UnityApp.exe"
                });
            }
        }
    }

    class UnityHost : HwndHost
    {
        private Process _childProcess;
        private HandleRef _childHandleRef;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        public string AppPath { get; set; }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            var cmdline = $"-parentHWND {hwndParent.Handle}";
            _childProcess = Process.Start(AppPath, cmdline);

            while (true)
            {
                var hwndChild = User32.FindWindowEx(hwndParent.Handle, IntPtr.Zero, null, null);
                if (hwndChild != IntPtr.Zero)
                {
                    User32.SendMessage(hwndChild, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);

                    return _childHandleRef = new HandleRef(this, hwndChild);
                }
                Thread.Sleep(100);
            }
        }
        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _childProcess.Dispose();
        }
    }

    static class User32
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hParent, IntPtr hChildAfter, string pClassName, string pWindowName);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
    }
}
