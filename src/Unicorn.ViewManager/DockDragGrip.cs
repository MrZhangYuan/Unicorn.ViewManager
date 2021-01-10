using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.Utilities;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    internal class DragAbsoluteEventArgs : RoutedEventArgs
    {
        public DragAbsoluteEventArgs(RoutedEvent evt, Point point)
          : base(evt)
          => this.ScreenPoint = point;

        public Point ScreenPoint { get; private set; }
    }

    internal class DragAbsoluteCompletedEventArgs : DragAbsoluteEventArgs
    {
        public DragAbsoluteCompletedEventArgs(RoutedEvent evt, Point point, bool isCompleted)
          : base(evt, point)
          => this.IsCompleted = isCompleted;

        public bool IsCompleted { get; set; }
    }

    internal class DockDragGrip : ContentControl, INonClientArea
    {
        private Point originalScreenPoint;
        private Point lastScreenPoint;
        private bool movedDuringDrag;
        private HwndSource currentSource;

        private HwndSource CurrentSource
        {
            get => this.currentSource;
            set
            {
                if (this.currentSource == value)
                    return;
                if (this.currentSource != null)
                    this.currentSource.RemoveHook(new HwndSourceHook(this.WndProc));
                this.currentSource = value;
                if (this.currentSource == null)
                    return;
                this.currentSource.AddHook(new HwndSourceHook(this.WndProc));
            }
        }

        public static readonly RoutedEvent DragStartedEvent = EventManager.RegisterRoutedEvent("DragStarted", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DockDragGrip));
        public static readonly RoutedEvent DragAbsoluteEvent = EventManager.RegisterRoutedEvent("DragAbsolute", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteEventArgs>), typeof(DockDragGrip));
        public static readonly RoutedEvent DragCompletedAbsoluteEvent = EventManager.RegisterRoutedEvent("DragCompletedAbsolute", RoutingStrategy.Bubble, typeof(EventHandler<DragAbsoluteCompletedEventArgs>), typeof(DockDragGrip));
        public static readonly RoutedEvent DragDeltaEvent = Thumb.DragDeltaEvent.AddOwner(typeof(DockDragGrip));
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsDragging), typeof(bool), typeof(DockDragGrip), (PropertyMetadata)new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsDraggingProperty = DockDragGrip.IsDraggingPropertyKey.DependencyProperty;
        public static readonly DependencyProperty IsWindowTitleBarProperty = DependencyProperty.Register(nameof(IsWindowTitleBar), typeof(bool), typeof(DockDragGrip), (PropertyMetadata)new FrameworkPropertyMetadata(false));


        public bool IsDragging
        {
            get => (bool)this.GetValue(DockDragGrip.IsDraggingProperty);
            protected set => this.SetValue(DockDragGrip.IsDraggingPropertyKey, value);
        }


        /// <summary>
        /// IsAtFloatingWindow
        /// </summary>
        public bool IsWindowTitleBar
        {
            get => (bool)this.GetValue(DockDragGrip.IsWindowTitleBarProperty);
            set => this.SetValue(DockDragGrip.IsWindowTitleBarProperty, value);
        }
        static DockDragGrip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockDragGrip), new FrameworkPropertyMetadata(typeof(DockDragGrip)));
        }

        public DockDragGrip()
        {
            PresentationSource.AddSourceChangedHandler((IInputElement)this, new SourceChangedEventHandler(this.OnSourceChanged));
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (this.IsConnectedToPresentationSource())
            {
                this.BeginDragging(this.PointToScreen(e.GetPosition((IInputElement)this)));
            }
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!this.IsMouseCaptured || !this.IsDragging || !this.IsConnectedToPresentationSource())
                return;
            this.movedDuringDrag = true;
            Point screen = this.PointToScreen(e.GetPosition((IInputElement)this));
            this.RaiseEvent((RoutedEventArgs)new DragDeltaEventArgs(screen.X - this.lastScreenPoint.X, screen.Y - this.lastScreenPoint.Y));
            this.RaiseDragAbsolute(screen);
            if (this.IsOutsideSensitivity(screen))
            {
                this.RaiseDragStarted(this.originalScreenPoint);
            }
            this.lastScreenPoint = screen;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured && this.IsDragging && this.IsConnectedToPresentationSource())
            {
                this.lastScreenPoint = this.PointToScreen(e.GetPosition((IInputElement)this));
                this.CompleteDrag();
            }
            base.OnMouseLeftButtonUp(e);
        }

        private void BeginDragging(Point screenPoint)
        {
            if (!this.CaptureMouse())
                return;
            this.IsDragging = true;
            this.originalScreenPoint = screenPoint;
            this.lastScreenPoint = screenPoint;
            this.movedDuringDrag = false;
        }
        public void CancelDrag()
        {
            if (!this.IsDragging)
                return;
            this.ReleaseCapture();
            this.RaiseDragCompletedAbsolute(this.lastScreenPoint, false);
        }

        private void CompleteDrag()
        {
            if (!this.IsDragging)
                return;
            this.ReleaseCapture();
            this.RaiseDragCompletedAbsolute(this.lastScreenPoint, this.movedDuringDrag);
        }

        private void ReleaseCapture()
        {
            if (!this.IsDragging)
                return;
            this.ClearValue(DockDragGrip.IsDraggingPropertyKey);
            if (!this.IsMouseCaptured)
                return;
            this.ReleaseMouseCapture();
        }

        private bool IsOutsideSensitivity(Point point)
        {
            point.Offset(-this.originalScreenPoint.X, -this.originalScreenPoint.Y);
            return Math.Abs(point.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(point.Y) > SystemParameters.MinimumVerticalDragDistance;
        }
        protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsMouseCapturedChanged(e);
            if (this.IsMouseCaptured)
                return;
            this.CancelDrag();
        }
        protected void RaiseDragStarted(Point point) => this.RaiseEvent((RoutedEventArgs)new DragAbsoluteEventArgs(DockDragGrip.DragStartedEvent, point));

        internal void RaiseDragAbsolute(Point point) => this.RaiseEvent((RoutedEventArgs)new DragAbsoluteEventArgs(DockDragGrip.DragAbsoluteEvent, point));

        protected void RaiseDragCompletedAbsolute(Point point, bool isCompleted) => this.RaiseEvent((RoutedEventArgs)new DragAbsoluteCompletedEventArgs(DockDragGrip.DragCompletedAbsoluteEvent, point, isCompleted));



        private void OnSourceChanged(object sender, SourceChangedEventArgs args) => this.CurrentSource = args.NewSource as HwndSource;

        private IntPtr WndProc(
          IntPtr hWnd,
          int msg,
          IntPtr wParam,
          IntPtr lParam,
          ref bool handled)
        {
            switch (msg)
            {
                case 534:
                    this.WmMoving(ref handled);
                    break;
                case 561:
                    this.WmEnterSizeMove();
                    break;
                case 562:
                    this.WmExitSizeMove(ref handled);
                    break;
            }
            return IntPtr.Zero;
        }

        internal static Point GetMessagePoint()
        {
            int messagePos = NativeMethods.GetMessagePos();
            return new Point((double)NativeMethods.GetXLParam(messagePos), (double)NativeMethods.GetYLParam(messagePos));
        }

        private void WmEnterSizeMove()
        {
            if (!this.IsWindowTitleBar)
                return;
            this.movedDuringDrag = false;
            this.RaiseDragStarted(DockDragGrip.GetMessagePoint());
        }

        private void WmExitSizeMove(ref bool handled)
        {
            if (!this.IsWindowTitleBar)
                return;
            this.RaiseDragCompletedAbsolute(DockDragGrip.GetMessagePoint(), this.movedDuringDrag);
            handled = this.CurrentSource == null || this.CurrentSource.IsDisposed;
        }

        private void WmMoving(ref bool handled)
        {
            if (!this.IsWindowTitleBar)
                return;
            this.movedDuringDrag = true;
            this.RaiseDragAbsolute(DockDragGrip.GetMessagePoint());
            handled = this.CurrentSource == null || this.CurrentSource.IsDisposed;
        }

        int INonClientArea.HitTest(Point point) => this.IsWindowTitleBar ? 2 : 0;
    }
}
