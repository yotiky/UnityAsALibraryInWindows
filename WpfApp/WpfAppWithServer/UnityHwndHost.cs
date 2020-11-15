using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Interop;

namespace WpfAppWithServer
{

    public class UnityHwndHost : HwndHost
    {
        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern IntPtr GetWindowLong32(IntPtr hWnd, Int32 nIndex);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        internal static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, Int32 nIndex);
        internal const Int32 GWLP_USERDATA = -21;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        internal const UInt32 WM_CLOSE = 0x0010;

        private string programName;
        private string arguments;
        private Process process = null;
        private IntPtr unityHWND = IntPtr.Zero;

        private JobObject job;

        private const int WM_ACTIVATE = 0x0006;
        private readonly IntPtr WA_ACTIVE = new IntPtr(1);
        private readonly IntPtr WA_INACTIVE = new IntPtr(0);

        public UnityHwndHost(string programName, string arguments = "")
        {
            this.programName = programName;
            this.arguments = arguments;
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            Debug.WriteLine("Going to launch Unity at: " + this.programName + " " + this.arguments);
            process = new Process();
            process.StartInfo.FileName = programName;
            process.StartInfo.Arguments = arguments + (arguments.Length == 0 ? "" : " ") + "-parentHWND " + hwndParent.Handle;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForInputIdle();

            job = JobObject.CreateAsKillOnJobClose();
            job.AssignProcess(process);

            int repeat = 50;
            while (unityHWND == IntPtr.Zero && repeat-- > 0)
            {
                Thread.Sleep(100);
                EnumChildWindows(hwndParent.Handle, WindowEnum, IntPtr.Zero);
            }
            if (unityHWND == IntPtr.Zero)
                throw new Exception("Unable to find Unity window");
            Debug.WriteLine("Found Unity window: " + unityHWND);

            repeat += 150;
            while ((GetWindowLong(unityHWND, GWLP_USERDATA).ToInt32() & 1) == 0 && --repeat > 0)
            {
                Thread.Sleep(100);
                Debug.WriteLine("Waiting for Unity to initialize... " + repeat);
            }
            if (repeat == 0)
            {
                Debug.WriteLine("Timed out while waiting for Unity to initialize");
            }
            else
            {
                Debug.WriteLine("Unity initialized!");
            }

            ActivateUnityWindow();

            return new HandleRef(this, unityHWND);
        }

        private void ActivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero);
        }

        private void DeactivateUnityWindow()
        {
            SendMessage(unityHWND, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            if (unityHWND != IntPtr.Zero)
                throw new Exception("Found multiple Unity windows");
            unityHWND = hwnd;
            return 0;
        }
        private IntPtr GetWindowLong(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }
            return GetWindowLongPtr64(hWnd, nIndex);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Destroy();
        }

        public void Destroy()
        {
            Debug.WriteLine("Asking Unity to exit...");
            PostMessage(unityHWND, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            job.Dispose();
            job = null;
            process.Dispose();
            process = null;
            unityHWND = IntPtr.Zero;
        }
    }
}
