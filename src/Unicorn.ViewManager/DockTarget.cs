using System.Windows;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public enum DockOrientation
    {
        All,
        Horizontal,
        Vertical,
        HorizontalOutter,
        VerticalOutter,
        Outter,
        Center,
        None
    }

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
        public static readonly DependencyProperty DockOrientationProperty = DependencyProperty.RegisterAttached("DockOrientation", typeof(DockOrientation), typeof(DockTarget), new PropertyMetadata(DockOrientation.All));

        public static DockOrientation GetDockOrientation(DependencyObject obj)
        {
            return (DockOrientation)obj.GetValue(DockOrientationProperty);
        }

        public static void SetDockOrientation(DependencyObject obj, DockOrientation value)
        {
            obj.SetValue(DockOrientationProperty, value);
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
