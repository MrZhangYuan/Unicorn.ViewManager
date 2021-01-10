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
        public static readonly DependencyProperty AdornedElementProperty = DependencyProperty.Register(nameof(AdornedElement), typeof(FrameworkElement), typeof(DockAdorner), new PropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty DockDirectionProperty = DependencyProperty.Register(nameof(DockDirection), typeof(DockDirection), typeof(DockAdorner), new PropertyMetadata((object)DockDirection.Fill));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(System.Windows.Controls.Orientation?), typeof(DockAdorner), new PropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty AreOuterTargetsEnabledProperty = DependencyProperty.Register(nameof(AreOuterTargetsEnabled), typeof(bool), typeof(DockAdorner), (PropertyMetadata)new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty AreInnerTargetsEnabledProperty = DependencyProperty.Register(nameof(AreInnerTargetsEnabled), typeof(bool), typeof(DockAdorner), (PropertyMetadata)new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty IsInnerCenterTargetEnabledProperty = DependencyProperty.Register(nameof(IsInnerCenterTargetEnabled), typeof(bool), typeof(DockAdorner), (PropertyMetadata)new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty AreInnerSideTargetsEnabledProperty = DependencyProperty.Register(nameof(AreInnerSideTargetsEnabled), typeof(bool), typeof(DockAdorner), (PropertyMetadata)new FrameworkPropertyMetadata(true));

        public IntPtr OwnerHwnd { get; set; }

        public FrameworkElement AdornedElement
        {
            get => (FrameworkElement)this.GetValue(DockAdorner.AdornedElementProperty);
            set => this.SetValue(DockAdorner.AdornedElementProperty, (object)value);
        }

        public DockDirection DockDirection
        {
            get => (DockDirection)this.GetValue(DockAdorner.DockDirectionProperty);
            set => this.SetValue(DockAdorner.DockDirectionProperty, (object)value);
        }

        public Orientation? Orientation
        {
            get => (Orientation?)this.GetValue(DockAdorner.OrientationProperty);
            set => this.SetValue(DockAdorner.OrientationProperty, (object)value);
        }

        public bool AreOuterTargetsEnabled
        {
            get => (bool)this.GetValue(DockAdorner.AreOuterTargetsEnabledProperty);
            set => this.SetValue(DockAdorner.AreOuterTargetsEnabledProperty, value);
        }

        public bool AreInnerTargetsEnabled
        {
            get => (bool)this.GetValue(DockAdorner.AreInnerTargetsEnabledProperty);
            set => this.SetValue(DockAdorner.AreInnerTargetsEnabledProperty, value);
        }

        public bool IsInnerCenterTargetEnabled
        {
            get => (bool)this.GetValue(DockAdorner.IsInnerCenterTargetEnabledProperty);
            set => this.SetValue(DockAdorner.IsInnerCenterTargetEnabledProperty, value);
        }

        public bool AreInnerSideTargetsEnabled
        {
            get => (bool)this.GetValue(DockAdorner.AreInnerSideTargetsEnabledProperty);
            set => this.SetValue(DockAdorner.AreInnerSideTargetsEnabledProperty, value);
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
