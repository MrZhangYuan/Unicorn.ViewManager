using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unicorn.ViewManager
{
    public class DockSiteAdorner : DockAdorner
    {
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(nameof(IsHighlighted), typeof(bool), typeof(DockSiteAdorner), (PropertyMetadata)new FrameworkPropertyMetadata(false));

        static DockSiteAdorner() => FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSiteAdorner), (PropertyMetadata)new FrameworkPropertyMetadata((object)typeof(DockSiteAdorner)));

        public DockTarget AdornedDockTarget
        {
            get
            {
                if (this.AdornedElement != null)
                {
                    return this.AdornedElement;
                }

                DockAdornerWindow ancestor = this.FindAncestor<DockAdornerWindow>();
                if (ancestor == null)
                    return (DockTarget)null;
                return ancestor.AdornedElement == null ? null : ancestor.AdornedElement;
            }
        }

        public bool IsHighlighted
        {
            get => (bool)this.GetValue(DockSiteAdorner.IsHighlightedProperty);
            set => this.SetValue(DockSiteAdorner.IsHighlightedProperty, value);
        }
    }
}
