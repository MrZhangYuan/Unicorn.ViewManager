using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public class SplitterItem : ContentControl
    {
        static SplitterItem()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterItem), new FrameworkPropertyMetadata(typeof(SplitterItem)));
        }

        public SplitterItem()
        {

        }
    }

}
