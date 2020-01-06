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

namespace Unicorn.ViewManager
{
    internal class RichViewItem : ContentControl
    {
        static RichViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichViewItem), new FrameworkPropertyMetadata(typeof(RichViewItem)));
        }
    }
}
