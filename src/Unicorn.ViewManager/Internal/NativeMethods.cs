using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace Unicorn.ViewManager.Internal
{
    internal static class NativeMethods
    {
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        internal static Point GetCursorPos()
        {
            POINT point1 = new POINT() { x = 0, y = 0 };
            Point point2 = new Point();
            if (NativeMethods.GetCursorPos(ref point1))
            {
                point2.X = (double)point1.x;
                point2.Y = (double)point1.y;
            }
            return point2;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref POINT point);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hwnd, int nCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(
        IntPtr hwndParent,
        NativeMethods.EnumWindowsProc lpEnumFunc,
        IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumThreadWindows(
        uint dwThreadId,
        NativeMethods.EnumWindowsProc lpfn,
        IntPtr lParam);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("User32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
           IntPtr hWnd,
           IntPtr hWndInsertAfter,
           int x,
           int y,
           int cx,
           int cy,
           int flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetMessagePos();

        internal static int GetXLParam(int lParam) => NativeMethods.LoWord(lParam);

        internal static int GetYLParam(int lParam) => NativeMethods.HiWord(lParam);

        internal static int HiWord(int value) => (int)(short)(value >> 16);

        internal static int LoWord(int value) => (int)(short)(value & (int)ushort.MaxValue);
    }
}
