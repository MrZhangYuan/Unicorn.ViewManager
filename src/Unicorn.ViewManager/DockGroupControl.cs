using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    public class DockGroupControl : SplitterItemsControl
    {
        static DockGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockGroupControl), new FrameworkPropertyMetadata(typeof(DockGroupControl)));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DockItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DockItem();
        }
    }
}
