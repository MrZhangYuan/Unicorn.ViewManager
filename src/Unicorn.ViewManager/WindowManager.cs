using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace Unicorn.ViewManager
{
    class WindowInfo
    {
        public Window Window { get; }
        public WindowInfo(Window window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
        }
    }

    static class WindowManager
    {
        internal static readonly Dictionary<IntPtr, WindowInfo> _allWindowInfos = new Dictionary<IntPtr, WindowInfo>();

        public static void RegisterWindow(Window window)
        {
            if (window != null)
            {
                window.Closed += Window_Closed;

                var handle = new WindowInteropHelper(window).Handle;
                if (handle != IntPtr.Zero)
                {
                    if (!_allWindowInfos.ContainsKey(handle))
                    {
                        _allWindowInfos.Add(handle, new WindowInfo(window));
                    }
                }
                else
                {
                    window.SourceInitialized += Window_SourceInitialized;
                }
            }
        }

        private static void Window_SourceInitialized(object sender, EventArgs e)
        {
            Window win = (Window)sender;
            win.SourceInitialized -= Window_SourceInitialized;
            var handle = new WindowInteropHelper(win).Handle;
            if (handle != IntPtr.Zero)
            {
                if (!_allWindowInfos.ContainsKey(handle))
                {
                    _allWindowInfos.Add(handle, new WindowInfo(win));
                }
            }
        }
        private static void Window_Closed(object sender, EventArgs e)
        {
            Window win = (Window)sender;
            win.Closed -= Window_Closed;
            var handle = new WindowInteropHelper(win).Handle;
            _allWindowInfos.Remove(handle);
        }
    }
}
