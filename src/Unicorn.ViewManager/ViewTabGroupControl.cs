using System.Windows;

namespace Unicorn.ViewManager
{
    public class ViewTabGroupControl : TabGroupControl
    {
        static ViewTabGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewTabGroupControl), new FrameworkPropertyMetadata(typeof(ViewTabGroupControl)));
        }

        public override TabGroupControl CreateTabGroup()
        {
            return new ViewTabGroupControl();
        }
    }
}


