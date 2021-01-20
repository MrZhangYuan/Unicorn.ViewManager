using System.Windows;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public enum DockTargetType
    {
        Outside,
        Center,
        Both
    }

    public enum FindMode
    {
        First,
        Ignore,
        Always
    }

    public class DockTarget : Border
    {
        public static readonly DependencyProperty DockTargetTypeProperty = DependencyProperty.Register(nameof(DockTargetType), typeof(DockTargetType), typeof(DockTarget), (PropertyMetadata)new FrameworkPropertyMetadata((object)DockTargetType.Center));
        public static readonly DependencyProperty AdornmentTargetProperty = DependencyProperty.Register(nameof(AdornmentTarget), typeof(FrameworkElement), typeof(DockTarget));
        public static readonly DependencyProperty FindModeProperty = DependencyProperty.Register("FindMode", typeof(FindMode), typeof(DockTarget), new PropertyMetadata(FindMode.First));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation?), typeof(DockTarget), new PropertyMetadata(null));

        public static Orientation? GetOrientation(DependencyObject obj)
        {
            return (Orientation?)obj.GetValue(OrientationProperty);
        }

        public static void SetOrientation(DependencyObject obj, Orientation? value)
        {
            obj.SetValue(OrientationProperty, value);
        }

        public FindMode FindMode
        {
            get => (FindMode)GetValue(FindModeProperty);
            set => SetValue(FindModeProperty, value);
        }

        public DockTargetType DockTargetType
        {
            get => (DockTargetType)this.GetValue(DockTargetTypeProperty);
            set => this.SetValue(DockTargetTypeProperty, (object)value);
        }

        public FrameworkElement AdornmentTarget
        {
            get => (FrameworkElement)this.GetValue(AdornmentTargetProperty);
            set => this.SetValue(AdornmentTargetProperty, (object)value);
        }
    }
}
