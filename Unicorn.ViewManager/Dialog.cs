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
    public class Dialog : PopupItem
    {
        public bool IsDrugMove
        {
            get
            {
                return (bool)GetValue(IsDrugMoveProperty);
            }
            set
            {
                SetValue(IsDrugMoveProperty, value);
            }
        }
        public static readonly DependencyProperty IsDrugMoveProperty = DependencyProperty.Register("IsDrugMove", typeof(bool), typeof(Dialog), new PropertyMetadata(true));


        static Dialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Dialog), new FrameworkPropertyMetadata(typeof(Dialog)));
        }

        protected internal override PopupItemContainer GetContainer()
        {
            return new DialogContainer
            {
                Dialog = this
            };
        }
    }
}
