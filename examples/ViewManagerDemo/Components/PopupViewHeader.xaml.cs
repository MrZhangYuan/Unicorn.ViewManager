using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ViewManagerDemo.Components
{
    /// <summary>
    /// PopupViewHeader.xaml 的交互逻辑
    /// </summary>
    public partial class PopupViewHeader : UserControl
    {

        public object TitleContent
        {
            get
            {
                return (object)GetValue(TitleContentProperty);
            }
            set
            {
                SetValue(TitleContentProperty, value);
            }
        }
        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register("TitleContent", typeof(object), typeof(PopupViewHeader), new PropertyMetadata(null));

        public PopupViewHeader()
        {
            InitializeComponent();
        }
    }
}
