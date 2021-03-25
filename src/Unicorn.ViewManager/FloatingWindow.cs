using System.Windows;

namespace Unicorn.ViewManager
{
    public class FloatingWindow : RichViewWindow
    {
        static FloatingWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingWindow), new FrameworkPropertyMetadata(typeof(FloatingWindow)));
        }

        internal FloatingWindow()
        {

        }
    }
}
