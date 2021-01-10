using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    public class SplitterResizePreviewWindow : Control
    {
        private HwndSource hwndSource;

        static SplitterResizePreviewWindow()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterResizePreviewWindow), new FrameworkPropertyMetadata(typeof(SplitterResizePreviewWindow)));
        }
        public void Move(double deviceLeft, double deviceTop)
        {
            if (hwndSource != null)
            {
                NativeMethods.SetWindowPos(hwndSource.Handle, IntPtr.Zero, (int)deviceLeft, (int)deviceTop, 0, 0, 85);
            }
        }
        public void Show(UIElement parentElement)
        {
            IntPtr owner = (PresentationSource.FromVisual(parentElement) as HwndSource)?.Handle ?? IntPtr.Zero;
            EnsureWindow(owner);
            base.Width = parentElement.RenderSize.Width;
            base.Height = parentElement.RenderSize.Height;
            Point point = parentElement.PointToScreen(new Point(0.0, 0.0));
            Size size = parentElement.RenderSize;
            NativeMethods.SetWindowPos(hwndSource.Handle, IntPtr.Zero, (int)point.X, (int)point.Y, (int)size.Width, (int)size.Height, 84);
        }
        public void Hide()
        {
            using (this.hwndSource)
            {
                this.hwndSource = null;
            }
        }
        private void EnsureWindow(IntPtr owner)
        {
            if (hwndSource == null)
            {
                HwndSourceParameters parameters = new HwndSourceParameters("SplitterResizePreviewWindow");
                int windowStyle = -2013265880;
                parameters.Width = 0;
                parameters.Height = 0;
                parameters.PositionX = 0;
                parameters.PositionY = 0;
                parameters.WindowStyle = windowStyle;
                parameters.UsesPerPixelOpacity = true;
                parameters.ParentWindow = owner;
                hwndSource = new HwndSource(parameters);
                hwndSource.SizeToContent = SizeToContent.WidthAndHeight;
                hwndSource.RootVisual = this;
            }
        }
    }

}
