using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    [TemplatePart(Name = "PART_HwndHost", Type = typeof(HwndHost))]
    public class AutoHideWindow : ContentControl, IResizable, IDisposable
    {
        public static readonly DependencyProperty IsAutoHiddenProperty = DependencyProperty.RegisterAttached("IsAutoHidden", typeof(bool), typeof(AutoHideWindow), (PropertyMetadata)new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));
        private bool disposed;

        static AutoHideWindow()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideWindow), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(AutoHideWindow)));
        }

        public AutoHideWindow()
        {
            AutoHideWindow.SetIsAutoHidden(this, true);
        }

        public static bool GetIsAutoHidden(UIElement element)
        {
            return element != null ? (bool)element.GetValue(AutoHideWindow.IsAutoHiddenProperty) : throw new ArgumentNullException(nameof(element));
        }

        public static void SetIsAutoHidden(UIElement element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(AutoHideWindow.IsAutoHiddenProperty, value);
        }

        public void UpdateBounds(
          double leftDelta,
          double topDelta,
          double widthDelta,
          double heightDelta)
        {
            Rect old = new Rect(0.0, 0.0, this.ActualWidth, this.ActualHeight);

            Rect newrect = old.Resize(
                    new Vector(leftDelta, topDelta),
                    new Vector(widthDelta, heightDelta),
                    this.MinSize,
                    this.MaxSize
                );

            if (widthDelta != 0.0)
            {
                this.Width = newrect.Width;
            }
            if (heightDelta != 0.0)
            {
                this.Height = newrect.Height;
            }
        }

        public Size MinSize
        {
            get => new Size(this.MinWidth.IsNonreal() ? 0.0 : this.MinWidth, this.MinHeight.IsNonreal() ? 0.0 : this.MinHeight);
        }

        public Size MaxSize
        {
            get => new Size(this.MaxWidth.IsNonreal() ? double.MaxValue : this.MaxWidth, this.MaxHeight.IsNonreal() ? double.MaxValue : this.MaxHeight);
        }

        public Rect CurrentScreenBounds
        {
            get
            {
                Point point = new Point(0.0, 0.0);
                if (this.IsConnectedToPresentationSource())
                {
                    point = this.PointToScreen(point);
                }
                return new Rect(point, DpiHelper.LogicalToDeviceUnits(this.RenderSize));
            }
        }

        public Rect CurrentBounds
        {
            get => new Rect(new Point(0.0, 0.0), this.RenderSize);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
                return;
            if (disposing && this.GetTemplateChild("PART_HwndHost") is IDisposable templateChild)
                templateChild.Dispose();
            this.disposed = true;
        }
    }
}
