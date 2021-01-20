using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.Utilities;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public class DockItem : TabGroupTabItem
    {
        public object Title
        {
            get
            {
                return (object)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(DockItem), new PropertyMetadata(null));


        static DockItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(typeof(DockItem)));
        }
    }
}
