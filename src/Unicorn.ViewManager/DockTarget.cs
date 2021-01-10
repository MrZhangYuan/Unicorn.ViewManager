using System.Windows;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public enum DockTargetType
    {
        Outside,
        Inside,
        SidesOnly,
        CenterOnly,
        Auto,
        FillPreview,
        InsertTabPreview
    }
    public enum DockSiteType
    {
        Default,
        NonDraggable,
    }
    public class DockTarget : Border
    {
        public static readonly DependencyProperty DockTargetTypeProperty = DependencyProperty.Register(nameof(DockTargetType), typeof(DockTargetType), typeof(DockTarget), (PropertyMetadata)new FrameworkPropertyMetadata((object)DockTargetType.Inside));
        public static readonly DependencyProperty DockSiteTypeProperty = DependencyProperty.Register(nameof(DockSiteType), typeof(DockSiteType), typeof(DockTarget), (PropertyMetadata)new FrameworkPropertyMetadata((object)DockSiteType.Default));
        public static readonly DependencyProperty AdornmentTargetProperty = DependencyProperty.Register(nameof(AdornmentTarget), typeof(FrameworkElement), typeof(DockTarget));

        public DockTargetType DockTargetType
        {
            get => (DockTargetType)this.GetValue(DockTarget.DockTargetTypeProperty);
            set => this.SetValue(DockTarget.DockTargetTypeProperty, (object)value);
        }

        public DockSiteType DockSiteType
        {
            get => (DockSiteType)this.GetValue(DockTarget.DockSiteTypeProperty);
            set => this.SetValue(DockTarget.DockSiteTypeProperty, (object)value);
        }

        public FrameworkElement AdornmentTarget
        {
            get => (FrameworkElement)this.GetValue(DockTarget.AdornmentTargetProperty);
            set => this.SetValue(DockTarget.AdornmentTargetProperty, (object)value);
        }
    }

}
