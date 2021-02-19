using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Unicorn.ViewManager
{
    public enum WindowResizeGripMode
    {
        Splitter = 1,
        DirectUpdate
    }
    public enum WindowResizeGripDirection
    {
        Left,
        Right,
        Top,
        Bottom,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
    public static class WindowResizeGripDirectionExtensions
    {
        public static bool IsResizingLeft(this WindowResizeGripDirection direction)
        {
            if (direction != 0 && direction != WindowResizeGripDirection.TopLeft)
            {
                return direction == WindowResizeGripDirection.BottomLeft;
            }
            return true;
        }

        public static bool IsResizingRight(this WindowResizeGripDirection direction)
        {
            if (direction != WindowResizeGripDirection.Right && direction != WindowResizeGripDirection.TopRight)
            {
                return direction == WindowResizeGripDirection.BottomRight;
            }
            return true;
        }

        public static bool IsResizingTop(this WindowResizeGripDirection direction)
        {
            if (direction != WindowResizeGripDirection.Top && direction != WindowResizeGripDirection.TopLeft)
            {
                return direction == WindowResizeGripDirection.TopRight;
            }
            return true;
        }

        public static bool IsResizingBottom(this WindowResizeGripDirection direction)
        {
            if (direction != WindowResizeGripDirection.Bottom && direction != WindowResizeGripDirection.BottomLeft)
            {
                return direction == WindowResizeGripDirection.BottomRight;
            }
            return true;
        }

        public static bool IsResizingHorizontally(this WindowResizeGripDirection direction)
        {
            if (direction != WindowResizeGripDirection.Top)
            {
                return direction != WindowResizeGripDirection.Bottom;
            }
            return false;
        }

        public static bool IsResizingVertically(this WindowResizeGripDirection direction)
        {
            if (direction != 0)
            {
                return direction != WindowResizeGripDirection.Right;
            }
            return false;
        }
    }


    public class WindowResizeGrip : Thumb
    {
        public static readonly DependencyProperty ResizeGripModeProperty;

        public static readonly DependencyProperty ResizeGripDirectionProperty;

        public static readonly DependencyProperty ResizeTargetProperty;

        private SplitterResizePreviewWindow currentPreviewWindow;

        private Rect initialTargetRect;

        public WindowResizeGripMode ResizeGripMode
        {
            get
            {
                return (WindowResizeGripMode)GetValue(ResizeGripModeProperty);
            }
            set
            {
                SetValue(ResizeGripModeProperty, value);
            }
        }

        public WindowResizeGripDirection ResizeGripDirection
        {
            get
            {
                return (WindowResizeGripDirection)GetValue(ResizeGripDirectionProperty);
            }
            set
            {
                SetValue(ResizeGripDirectionProperty, value);
            }
        }

        public IResizable ResizeTarget
        {
            get
            {
                return (IResizable)GetValue(ResizeTargetProperty);
            }
            set
            {
                SetValue(ResizeTargetProperty, value);
            }
        }

        private bool IsShowingResizePreview => currentPreviewWindow != null;

        static WindowResizeGrip()
        {
            ResizeGripModeProperty = DependencyProperty.Register("ResizeGripMode", typeof(WindowResizeGripMode), typeof(WindowResizeGrip), new PropertyMetadata(WindowResizeGripMode.Splitter));
            ResizeGripDirectionProperty = DependencyProperty.Register("ResizeGripDirection", typeof(WindowResizeGripDirection), typeof(WindowResizeGrip));
            ResizeTargetProperty = DependencyProperty.Register("ResizeTarget", typeof(IResizable), typeof(WindowResizeGrip));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowResizeGrip), new FrameworkPropertyMetadata(typeof(WindowResizeGrip)));
        }

        public WindowResizeGrip()
        {
            AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStarted));
            AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));
        }

        private void OnDragStarted(object sender, DragStartedEventArgs args)
        {
            if (ResizeTarget != null)
            {
                initialTargetRect = ResizeTarget.CurrentBounds;
                if (ResizeGripMode == WindowResizeGripMode.Splitter)
                {
                    currentPreviewWindow = new SplitterResizePreviewWindow();
                    currentPreviewWindow.Show(this);
                }
            }
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs args)
        {
            if (ResizeTarget != null && PresentationSource.FromVisual(this) != null)
            {
                switch (ResizeGripMode)
                {
                    case WindowResizeGripMode.Splitter:
                        if (IsShowingResizePreview)
                        {
                            Rect currentScreenBounds = ResizeTarget.CurrentScreenBounds;
                            Size size = ResizeTarget.MinSize;
                            Size size2 = ResizeTarget.MaxSize;
                            Size size3 = base.RenderSize;
                            Point point = PointToScreen(new Point(0.0, 0.0));
                            if (ResizeGripDirection.IsResizingHorizontally())
                            {
                                point.X += args.HorizontalChange;
                            }
                            if (ResizeGripDirection.IsResizingVertically())
                            {
                                point.Y += args.VerticalChange;
                            }
                            if (ResizeGripDirection.IsResizingRight())
                            {
                                point.X = Math.Max(currentScreenBounds.Left + size.Width - size3.Width, Math.Min(currentScreenBounds.Left + size2.Width - size3.Width, point.X));
                            }
                            if (ResizeGripDirection.IsResizingLeft())
                            {
                                point.X = Math.Max(currentScreenBounds.Right - size2.Width, Math.Min(currentScreenBounds.Right - size.Width, point.X));
                            }
                            if (ResizeGripDirection.IsResizingBottom())
                            {
                                point.Y = Math.Max(currentScreenBounds.Top + size.Height - size3.Height, Math.Min(currentScreenBounds.Top + size2.Height - size3.Height, point.Y));
                            }
                            if (ResizeGripDirection.IsResizingTop())
                            {
                                point.Y = Math.Max(currentScreenBounds.Bottom - size2.Height, Math.Min(currentScreenBounds.Bottom - size.Height, point.Y));
                            }
                            currentPreviewWindow.Move(point.X, point.Y);
                        }
                        break;
                    case WindowResizeGripMode.DirectUpdate:
                        UpdateTargetBounds(args);
                        break;
                }
            }
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs args)
        {
            if (ResizeTarget != null)
            {
                WindowResizeGripMode resizeGripMode = ResizeGripMode;
                if (resizeGripMode == WindowResizeGripMode.Splitter && IsShowingResizePreview)
                {
                    currentPreviewWindow.Hide();
                    currentPreviewWindow = null;
                    UpdateTargetBounds(args);
                }
            }
        }

        private void UpdateTargetBounds(DragDeltaEventArgs args)
        {
            Point change = new Point(args.HorizontalChange, args.VerticalChange);
            UpdateTargetBounds(change);
        }

        private void UpdateTargetBounds(DragCompletedEventArgs args)
        {
            Point change = new Point(args.HorizontalChange, args.VerticalChange);
            UpdateTargetBounds(change);
        }

        private void UpdateTargetBounds(Point change)
        {
            if (change.X != 0.0 || change.Y != 0.0)
            {
                switch (ResizeGripDirection)
                {
                    case WindowResizeGripDirection.Bottom:
                        ResizeTarget.UpdateBounds(0.0, 0.0, 0.0, change.Y);
                        break;
                    case WindowResizeGripDirection.Right:
                        ResizeTarget.UpdateBounds(0.0, 0.0, change.X, 0.0);
                        break;
                    case WindowResizeGripDirection.Left:
                        ResizeTarget.UpdateBounds(change.X, 0.0, 0.0 - change.X, 0.0);
                        break;
                    case WindowResizeGripDirection.Top:
                        ResizeTarget.UpdateBounds(0.0, change.Y, 0.0, 0.0 - change.Y);
                        break;
                    case WindowResizeGripDirection.BottomRight:
                        ResizeTarget.UpdateBounds(0.0, 0.0, change.X, change.Y);
                        break;
                    case WindowResizeGripDirection.BottomLeft:
                        ResizeTarget.UpdateBounds(change.X, 0.0, 0.0 - change.X, change.Y);
                        break;
                    case WindowResizeGripDirection.TopRight:
                        ResizeTarget.UpdateBounds(0.0, change.Y, change.X, 0.0 - change.Y);
                        break;
                    case WindowResizeGripDirection.TopLeft:
                        ResizeTarget.UpdateBounds(change.X, change.Y, 0.0 - change.X, 0.0 - change.Y);
                        break;
                }
            }
        }
    }
}
