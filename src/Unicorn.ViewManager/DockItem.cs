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

namespace Unicorn.ViewManager
{
    public interface IDockHost
    {

    }

    internal class DockItem : SplitterItem
    {
        static DockItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockItem), new FrameworkPropertyMetadata(typeof(DockItem)));
        }
    }
}
