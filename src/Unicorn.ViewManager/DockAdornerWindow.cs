using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    public class DockAdornerWindow : ContentControl
    {
        private IntPtr ownerHwnd;
        private DockTarget dockTarget;
        private HwndSource window;
        private DockAdorner innerContent;
        private const double OutAdornmentOffset = 10.0;

        public DockAdornerWindow(IntPtr ownerHwnd) => this.ownerHwnd = ownerHwnd;

        public bool IsDockGroup => this.DockDirection == DockDirection.Fill;

        public FrameworkElement AdornedElement { get; set; }

        public DockDirection DockDirection { get; set; }

        public Orientation? Orientation { get; set; }

        public bool AreOuterTargetsEnabled { get; set; }

        public bool AreInnerTargetsEnabled { get; set; }

        public bool IsInnerCenterTargetEnabled { get; set; }

        public bool AreInnerSideTargetsEnabled { get; set; }

        public void PrepareAndShow()
        {
            DockTarget adornedElement = this.AdornedElement as DockTarget;
            if (this.dockTarget != adornedElement)
            {
                this.PrepareAndHide();
                this.dockTarget = adornedElement;
            }
            if (this.window == null)
            {
                this.UpdateContent();
                this.window = new HwndSource(new HwndSourceParameters()
                {
                    Width = 0,
                    Height = 0,
                    ParentWindow = this.ownerHwnd,
                    UsesPerPixelOpacity = true,
                    WindowName = nameof(DockAdornerWindow),
                    WindowStyle = -2013265880
                });
                this.window.SizeToContent = SizeToContent.WidthAndHeight;
                this.window.RootVisual = (Visual)this;
                DockManager.Instance.RegisterDockSite((Visual)this, this.window.Handle);
            }
            this.UpdatePositionAndVisibility();
        }

        public void PrepareAndHide()
        {
            if (this.window == null)
                return;
            DockManager.Instance.UnregisterDockSite((Visual)this);
            this.Content = (object)(this.innerContent = (DockAdorner)null);
            this.window.Dispose();
            this.window = (HwndSource)null;
        }

        private void UpdatePositionAndVisibility()
        {
            if (!this.IsArrangeValid)
                this.UpdateLayout();
            double actualWidth = this.ActualWidth;
            double actualHeight = this.ActualHeight;
            double num1 = actualWidth - this.AdornedElement.ActualWidth;
            double num2 = actualHeight - this.AdornedElement.ActualHeight;
            Point logicalUnits = DpiHelper.DeviceToLogicalUnits(this.AdornedElement.PointToScreen(new Point(0.0, 0.0)));
            RECT lpRect;
            NativeMethods.GetWindowRect(this.ownerHwnd, out lpRect);
            Point point2 = new Point((double)lpRect.Left, (double)lpRect.Top);
            Vector vector = Point.Subtract(logicalUnits, point2);
            double num3 = vector.X - num1 / 2.0;
            double num4 = vector.Y - num2 / 2.0;
            double num5 = vector.X + OutAdornmentOffset;
            double num6 = vector.X - actualWidth + this.AdornedElement.ActualWidth - OutAdornmentOffset;
            double num7 = vector.Y + OutAdornmentOffset;
            double num8 = vector.Y - actualHeight + this.AdornedElement.ActualHeight - OutAdornmentOffset;
            double offsetX = 0.0;
            double offsetY = 0.0;
            switch (this.DockDirection)
            {
                case DockDirection.Fill:
                    offsetX = num3;
                    offsetY = num4;
                    break;
                case DockDirection.Left:
                    offsetX = num5;
                    offsetY = num4;
                    break;
                case DockDirection.Top:
                    offsetX = num3;
                    offsetY = num7;
                    break;
                case DockDirection.Right:
                    offsetX = num6;
                    offsetY = num4;
                    break;
                case DockDirection.Bottom:
                    offsetX = num3;
                    offsetY = num8;
                    break;
            }
            point2.Offset(offsetX, offsetY);
            Point deviceUnits = DpiHelper.LogicalToDeviceUnits(point2);
            NativeMethods.SetWindowPos(this.window.Handle, IntPtr.Zero, (int)deviceUnits.X, (int)deviceUnits.Y, 0, 0, 85);
        }

        private void UpdateContent()
        {
            DockAdorner dockAdorner = !this.IsDockGroup ? (DockAdorner)new DockSiteAdorner() : (DockAdorner)new DockGroupAdorner();
            dockAdorner.AdornedElement = this.AdornedElement;
            dockAdorner.DockDirection = this.DockDirection;
            dockAdorner.Orientation = this.Orientation;
            dockAdorner.AreOuterTargetsEnabled = this.AreOuterTargetsEnabled;
            dockAdorner.AreInnerTargetsEnabled = this.AreInnerTargetsEnabled;
            dockAdorner.IsInnerCenterTargetEnabled = this.IsInnerCenterTargetEnabled;
            dockAdorner.AreInnerSideTargetsEnabled = this.AreInnerSideTargetsEnabled;
            this.Content = (object)(this.innerContent = dockAdorner);
            this.innerContent.UpdateContent();
        }
    }

}
