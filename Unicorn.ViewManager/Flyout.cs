using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Unicorn.ViewManager
{
    public enum FlyoutLocation
    {
        Left,
        Right,
        Bottom,
        Top
    }
    public class Flyout : PopupItem
    {
        public FlyoutLocation FlyoutLocation
        {
            get
            {
                return (FlyoutLocation)GetValue(FlyoutLocationProperty);
            }
            set
            {
                SetValue(FlyoutLocationProperty, value);
            }
        }
        public static readonly DependencyProperty FlyoutLocationProperty = DependencyProperty.Register("FlyoutLocation", typeof(FlyoutLocation), typeof(Flyout), new PropertyMetadata(FlyoutLocation.Left));

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        protected internal override PopupItemContainer GetContainer()
        {
            return new FlyoutContainer
            {
                Flyout = this
            };
        }
    }
}
