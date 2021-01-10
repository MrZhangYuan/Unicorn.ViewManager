using System.Windows;

namespace Unicorn.ViewManager
{
    public class DockGroupAdorner : DockAdorner
    {
        public static readonly DependencyProperty DockSiteTypeProperty = DependencyProperty.Register(nameof(DockSiteType), typeof(DockSiteType), typeof(DockGroupAdorner), new PropertyMetadata((object)DockSiteType.Default));
        public static readonly DependencyProperty IsFirstProperty = DependencyProperty.Register(nameof(IsFirst), typeof(bool), typeof(DockGroupAdorner), (PropertyMetadata)new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsLastProperty = DependencyProperty.Register(nameof(IsLast), typeof(bool), typeof(DockGroupAdorner), (PropertyMetadata)new FrameworkPropertyMetadata(false));

        static DockGroupAdorner() => FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DockGroupAdorner), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(DockGroupAdorner)));

        public DockSiteType DockSiteType
        {
            get => (DockSiteType)this.GetValue(DockGroupAdorner.DockSiteTypeProperty);
            set => this.SetValue(DockGroupAdorner.DockSiteTypeProperty, (object)value);
        }

        public bool IsFirst
        {
            get => (bool)this.GetValue(DockGroupAdorner.IsFirstProperty);
            set => this.SetValue(DockGroupAdorner.IsFirstProperty, value);
        }

        public bool IsLast
        {
            get => (bool)this.GetValue(DockGroupAdorner.IsLastProperty);
            set => this.SetValue(DockGroupAdorner.IsLastProperty, value);
        }

        protected override void UpdateContentCore()
        {
            base.UpdateContentCore();
            if (!(this.AdornedElement is DockTarget adornedElement))
                return;
            this.DockSiteType = adornedElement.DockSiteType;
            SplitterItem ancestor = adornedElement.FindAncestor<SplitterItem>();
            if (ancestor == null)
                return;
            this.IsFirst = SplitterPanel.GetIsFirst((UIElement)ancestor);
            this.IsLast = SplitterPanel.GetIsLast((UIElement)ancestor);
        }
    }

}
