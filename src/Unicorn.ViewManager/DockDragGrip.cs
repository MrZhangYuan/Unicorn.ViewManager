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
    public class DragAbsoluteEventArgs : RoutedEventArgs
    {
        public DragAbsoluteEventArgs(RoutedEvent evt, Point point)
          : base(evt)
        {
            this.ScreenPoint = point;
        }

        public Point ScreenPoint
        {
            get;
            private set;
        }
    }

    public class DragAbsoluteCompletedEventArgs : DragAbsoluteEventArgs
    {
        public DragAbsoluteCompletedEventArgs(RoutedEvent evt, Point point, bool isCompleted)
          : base(evt, point)
        {
            this.IsCompleted = isCompleted;
        }

        public bool IsCompleted
        {
            get;
            set;
        }
    }

    public class DockDragGrip : ContentControl, INonClientArea
    {
        private Point _originalScreenPoint;
        private Point _lastScreenPoint;
        private bool _movedDuringDrag;
        private HwndSource _currentSource;

        private HwndSource CurrentSource
        {
            get => this._currentSource;
            set
            {
                if (this._currentSource == value)
                {
                    return;
                }

                if (this._currentSource != null)
                {
                    this._currentSource.RemoveHook(new HwndSourceHook(this.WndProc));
                }

                this._currentSource = value;

                if (this._currentSource != null)
                {
                    this._currentSource.AddHook(new HwndSourceHook(this.WndProc));
                }
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

        public bool IsWindowTitleBar
        {
            get => (bool)this.GetValue(DockDragGrip.IsWindowTitleBarProperty);
            set => this.SetValue(DockDragGrip.IsWindowTitleBarProperty, value);
        }


        public object Element
        {
            get => GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }

        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register("Element", typeof(object), typeof(DockDragGrip), new PropertyMetadata(null));


        static DockDragGrip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockDragGrip), new FrameworkPropertyMetadata(typeof(DockDragGrip)));
        }

        public DockDragGrip()
        {
            PresentationSource.AddSourceChangedHandler((IInputElement)this, new SourceChangedEventHandler(this.OnSourceChanged));

            //使用此种方式会导致里面的Button不执行命令
            //this.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.MouseLeftButtonDown_Handle), true);
            //this.AddHandler(MouseMoveEvent, new MouseEventHandler(this.MouseMove_Handle), true);
            //this.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(this.MouseLeftButtonUp_Handle), true);
        }

        //private void MouseLeftButtonDown_Handle(object sender, MouseButtonEventArgs e)
        //{
        //    if (this.IsConnectedToPresentationSource())
        //    {
        //        this.BeginDragging(this.PointToScreen(e.GetPosition((IInputElement)this)));
        //    }
        //    base.OnMouseLeftButtonDown(e);
        //}

        //private void MouseMove_Handle(object sender, MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    if (!this.IsMouseCaptured || !this.IsDragging || !this.IsConnectedToPresentationSource())
        //        return;
        //    this._movedDuringDrag = true;
        //    Point screen = this.PointToScreen(e.GetPosition((IInputElement)this));
        //    this.RaiseEvent((RoutedEventArgs)new DragDeltaEventArgs(screen.X - this._lastScreenPoint.X, screen.Y - this._lastScreenPoint.Y));
        //    this.RaiseDragAbsolute(screen);
        //    if (this.IsOutsideSensitivity(screen))
        //    {
        //        this.RaiseDragStarted(this._originalScreenPoint);
        //    }
        //    this._lastScreenPoint = screen;
        //}

        //private void MouseLeftButtonUp_Handle(object sender, MouseButtonEventArgs e)
        //{
        //    if (this.IsMouseCaptured && this.IsDragging && this.IsConnectedToPresentationSource())
        //    {
        //        this._lastScreenPoint = this.PointToScreen(e.GetPosition((IInputElement)this));
        //        this.CompleteDrag();
        //    }
        //    base.OnMouseLeftButtonUp(e);
        //}


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
            this._movedDuringDrag = true;
            Point screen = this.PointToScreen(e.GetPosition((IInputElement)this));
            this.RaiseEvent((RoutedEventArgs)new DragDeltaEventArgs(screen.X - this._lastScreenPoint.X, screen.Y - this._lastScreenPoint.Y));
            this.RaiseDragAbsolute(screen);
            if (this.IsOutsideSensitivity(screen))
            {
                this.RaiseDragStarted(this._originalScreenPoint);
            }
            this._lastScreenPoint = screen;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured && this.IsDragging && this.IsConnectedToPresentationSource())
            {
                this._lastScreenPoint = this.PointToScreen(e.GetPosition((IInputElement)this));
                this.CompleteDrag();
            }
            base.OnMouseLeftButtonUp(e);
        }


        private void BeginDragging(Point screenPoint)
        {
            if (!this.CaptureMouse())
                return;
            this.IsDragging = true;
            this._originalScreenPoint = screenPoint;
            this._lastScreenPoint = screenPoint;
            this._movedDuringDrag = false;
        }

        public void CancelDrag()
        {
            if (!this.IsDragging)
                return;
            this.ReleaseCapture();
            this.RaiseDragCompletedAbsolute(this._lastScreenPoint, false);
        }

        private void CompleteDrag()
        {
            if (!this.IsDragging)
                return;
            this.ReleaseCapture();
            this.RaiseDragCompletedAbsolute(this._lastScreenPoint, this._movedDuringDrag);
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
            point.Offset(-this._originalScreenPoint.X, -this._originalScreenPoint.Y);
            return Math.Abs(point.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(point.Y) > SystemParameters.MinimumVerticalDragDistance;
        }

        protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsMouseCapturedChanged(e);
            if (this.IsMouseCaptured)
                return;
            this.CancelDrag();
        }

        protected void RaiseDragStarted(Point point)
        {
            this.RaiseEvent((RoutedEventArgs)new DragAbsoluteEventArgs(DockDragGrip.DragStartedEvent, point));
        }

        internal void RaiseDragAbsolute(Point point)
        {
            this.RaiseEvent((RoutedEventArgs)new DragAbsoluteEventArgs(DockDragGrip.DragAbsoluteEvent, point));
        }

        protected void RaiseDragCompletedAbsolute(Point point, bool isCompleted)
        {
            this.RaiseEvent((RoutedEventArgs)new DragAbsoluteCompletedEventArgs(DockDragGrip.DragCompletedAbsoluteEvent, point, isCompleted));
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs args)
        {
            this.CurrentSource = args.NewSource as HwndSource;
        }

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
            this._movedDuringDrag = false;
            this.RaiseDragStarted(DockDragGrip.GetMessagePoint());
        }

        private void WmExitSizeMove(ref bool handled)
        {
            if (!this.IsWindowTitleBar)
                return;
            this.RaiseDragCompletedAbsolute(DockDragGrip.GetMessagePoint(), this._movedDuringDrag);
            handled = this.CurrentSource == null || this.CurrentSource.IsDisposed;
        }

        private void WmMoving(ref bool handled)
        {
            if (!this.IsWindowTitleBar)
                return;
            this._movedDuringDrag = true;
            this.RaiseDragAbsolute(DockDragGrip.GetMessagePoint());
            handled = this.CurrentSource == null || this.CurrentSource.IsDisposed;
        }

        int INonClientArea.HitTest(Point point)
        {
            return this.IsWindowTitleBar ? 2 : 0;
        }
    }
}
