using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinformsEngine.Engine.Systems
{
    public class IdleTimer
    {
        public event Action<float> Tick;
        private const uint PM_NOREMOVE = 0x0000;
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private TimeSpan _lastTIme;

        public void Start()
        {
            _stopWatch.Start();
            _lastTIme = _stopWatch.Elapsed;
            Application.Idle += OnApplicationIdle;
        }

        public void Stop()
        {
            Application.Idle -= OnApplicationIdle;
            _stopWatch.Stop();
        }

        public void OnApplicationIdle(object sender, EventArgs e)
        {
            while (IsIdle())
            {
                TimeSpan currentTime = _stopWatch.Elapsed;
                float delta = (float)(currentTime - _lastTIme).TotalSeconds;
                Tick?.Invoke(delta);
                _lastTIme = currentTime;
            }
        }

        private bool IsIdle()
        {
            return !PeekMessage(out NativeMessage msg, IntPtr.Zero, 0, 0, PM_NOREMOVE);
        }

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(
            out NativeMessage lpMsg,
            IntPtr hWnd,
            uint wMsgFilterMin,
            uint wMsgFilterMax,
            uint wRemoveMsg);

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }
    }
}