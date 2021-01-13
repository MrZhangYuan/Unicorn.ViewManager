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
        private const double OutAdornmentOffset = 10.0;

        private IntPtr _ownerHwnd;
        private DockTarget _dockTarget;
        private HwndSource _window;
        private DockAdorner _innerContent;

        public IntPtr Handle
        {
            get => _window == null ? IntPtr.Zero : _window.Handle;
        }

        public bool IsDockGroup
        {
            get => this.DockDirection == DockDirection.Fill;
        }

        public DockTarget AdornedElement
        {
            get;
            set;
        }

        /// <summary>
        /// DockAdorner 的位置，上 - 下 - 左 - 右 - 中
        /// </summary>
        public DockDirection DockDirection
        {
            get;
            set;
        }

        /// <summary>
        /// 停靠的方向
        /// Horizontal：上下停靠禁用
        /// Vertical：左右停靠禁用
        /// NULL：全启用
        /// </summary>
        public Orientation? Orientation
        {
            get;
            set;
        }

        /// <summary>
        /// 靠近最中间的填充的外层第一层
        /// </summary>
        public bool AreInnerTargetsEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// 最中间的填充是否启用
        /// </summary>
        public bool IsInnerCenterTargetEnabled
        {
            get;
            set;
        }

        public DockAdornerWindow(IntPtr ownerHwnd)
        {
            this._ownerHwnd = ownerHwnd;
        }

        public void PrepareAndShow()
        {
            DockTarget adornedElement = this.AdornedElement as DockTarget;
            if (this._dockTarget != adornedElement)
            {
                this.PrepareAndHide();
                this._dockTarget = adornedElement;
            }
            if (this._window == null)
            {
                this.UpdateContent();
                this._window = new HwndSource(new HwndSourceParameters()
                {
                    Width = 0,
                    Height = 0,
                    ParentWindow = this._ownerHwnd,
                    UsesPerPixelOpacity = true,
                    WindowName = nameof(DockAdornerWindow),
                    WindowStyle = -2013265880
                });
                this._window.SizeToContent = SizeToContent.WidthAndHeight;
                this._window.RootVisual = (Visual)this;
                DockManager.RegisterDockSite((Visual)this, this._window.Handle);
            }
            this.UpdatePositionAndVisibility();
        }

        public void PrepareAndHide()
        {
            if (this._window == null)
                return;
            DockManager.UnregisterDockSite((Visual)this);
            this.Content = (object)(this._innerContent = (DockAdorner)null);
            this._window.Dispose();
            this._window = (HwndSource)null;
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
            NativeMethods.GetWindowRect(this._ownerHwnd, out lpRect);
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
            NativeMethods.SetWindowPos(this._window.Handle, IntPtr.Zero, (int)deviceUnits.X, (int)deviceUnits.Y, 0, 0, 85);
        }

        private void UpdateContent()
        {
            DockAdorner dockAdorner = !this.IsDockGroup ? (DockAdorner)new DockSiteAdorner() : (DockAdorner)new DockGroupAdorner();
            dockAdorner.AdornedElement = this.AdornedElement;
            dockAdorner.DockDirection = this.DockDirection;
            dockAdorner.Orientation = this.Orientation;
            dockAdorner.AreInnerTargetsEnabled = this.AreInnerTargetsEnabled;
            dockAdorner.IsInnerCenterTargetEnabled = this.IsInnerCenterTargetEnabled;
            this.Content = (object)(this._innerContent = dockAdorner);
            this._innerContent.UpdateContent();
        }
    }

}
