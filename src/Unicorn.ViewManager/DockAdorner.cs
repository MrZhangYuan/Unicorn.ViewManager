using System;
using System.Windows;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public enum DockDirection
    {
        Fill,
        Left,
        Top,
        Right,
        Bottom,
    }
    public class DockAdorner : ContentControl
    {
        public static readonly DependencyProperty AdornedElementProperty = DependencyProperty.Register(nameof(AdornedElement), typeof(DockTarget), typeof(DockAdorner), new PropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty DockDirectionProperty = DependencyProperty.Register(nameof(DockDirection), typeof(DockDirection), typeof(DockAdorner), new PropertyMetadata(DockDirection.Fill));
        public static readonly DependencyProperty DockOrientationProperty = DependencyProperty.Register(nameof(DockOrientation), typeof(DockOrientation), typeof(DockAdorner), new PropertyMetadata(DockOrientation.All));

        public IntPtr OwnerHwnd { get; set; }

        public DockTarget AdornedElement
        {
            get => (DockTarget)this.GetValue(DockAdorner.AdornedElementProperty);
            set => this.SetValue(DockAdorner.AdornedElementProperty, (object)value);
        }

        public DockDirection DockDirection
        {
            get => (DockDirection)this.GetValue(DockAdorner.DockDirectionProperty);
            set => this.SetValue(DockAdorner.DockDirectionProperty, (object)value);
        }

        public DockOrientation DockOrientation
        {
            get => (DockOrientation)this.GetValue(DockAdorner.DockOrientationProperty);
            set => this.SetValue(DockAdorner.DockOrientationProperty, (object)value);
        }

        public void UpdateContent()
        {
            this.UpdateContentCore();
            this.InvalidateArrange();
            this.UpdateLayout();
        }

        protected virtual void UpdateContentCore()
        {
        }
    }

}
