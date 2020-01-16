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
    public class DefaultPopupItemContainer : PopupItemContainer
    {
        static DefaultPopupItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DefaultPopupItemContainer), new FrameworkPropertyMetadata(typeof(DefaultPopupItemContainer)));
        }
    }
}
