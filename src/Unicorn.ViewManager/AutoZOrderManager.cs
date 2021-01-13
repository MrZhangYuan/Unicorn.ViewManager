using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    static class AutoZOrderManager
    {
        private static DispatcherTimer _autoZOrderTimer;
        private static Window _currentDragOverWindow;

        public static Window CurrentDragOverWindow
        {
            get => _currentDragOverWindow;
            set
            {
                if (_currentDragOverWindow == value)
                    return;
                StopTimer();
                _currentDragOverWindow = value;
                if (_currentDragOverWindow == null)
                    return;
                _autoZOrderTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, new EventHandler(AutoZOrderManager.OnAutoZOrderTimer), AutoZOrderManager._currentDragOverWindow.Dispatcher);
                _autoZOrderTimer.Start();
            }
        }

        public static void AdornersCleared(Window window)
        {
            if (CurrentDragOverWindow != window)
                return;
            CurrentDragOverWindow = (Window)null;
        }

        private static void OnAutoZOrderTimer(object obj, EventArgs args)
        {
            StopTimer();
            if (CurrentDragOverWindow == null
                || DockManager.CurrentDraggedContext.DraggedWindow == null)
            {
                return;
            }

            int num = 1;
            IntPtr draggedWindowHandle = new WindowInteropHelper(DockManager.CurrentDraggedContext.DraggedWindow).Handle;
            IntPtr handle = new WindowInteropHelper(CurrentDragOverWindow).Handle;
            foreach (Window floatingWindow in WindowManager._allWindowInfos.Values.Select(_p => _p.Window))
            {
                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(floatingWindow);
                if (windowInteropHelper.Handle != draggedWindowHandle && windowInteropHelper.Owner == handle)
                    ++num;
            }

            var older = GetWindowZOrder(CurrentDragOverWindow);
            if (older == num)
                return;

            NativeMethods.SetWindowPos(handle, draggedWindowHandle, 0, 0, 0, 0, 16403);

            if (DockManager.CurrentDraggedContext.DockDragGrip == null)
                return;

            DockManager.CurrentDraggedContext.CleanupAdorners(null);
            DockManager.CurrentDraggedContext.DockDragGrip.RaiseDragAbsolute(NativeMethods.GetCursorPos());
        }

        private static void StopTimer()
        {
            if (_autoZOrderTimer == null)
                return;
            _autoZOrderTimer.Stop();
            _autoZOrderTimer = null;
        }


        internal static int GetWindowZOrder(Window window) => GetWindowZOrder(window, true);

        private static int GetWindowZOrder(Window window, bool includeMainWindow)
        {
            var handles = WindowManager._allWindowInfos.Keys;

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            int num1 = 0;
            IntPtr num2 = !includeMainWindow ? new IntPtr(-1) : new WindowInteropHelper(Application.Current.MainWindow).Handle;
            while (hwnd != IntPtr.Zero)
            {
                hwnd = NativeMethods.GetWindow(hwnd, 3);
                if (hwnd != IntPtr.Zero && (num2 == hwnd || handles.Contains(hwnd)))
                    ++num1;
            }
            return num1;
        }

    }
}
