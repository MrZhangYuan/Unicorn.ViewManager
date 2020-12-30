using System.Windows;
using System.Windows.Media;

namespace Unicorn.ViewManager
{
    internal static class ViewTreeHelper
    {
        public static T FindParent<T>(FrameworkElement element) where T : class
        {
            if (element == null)
            {
                return null;
            }

            FrameworkElement parent = VisualTreeHelper.GetParent(element) as FrameworkElement;
            if (parent == null)
            {
                parent = LogicalTreeHelper.GetParent(element) as FrameworkElement;
            }

            while (parent != null
                && !(parent is T)
                && !(parent is Window))
            {
                var temp = VisualTreeHelper.GetParent(parent) as FrameworkElement;

                if (temp == null)
                {
                    temp = LogicalTreeHelper.GetParent(parent) as FrameworkElement;
                }

                parent = temp;
            }

            return parent as T;
        }
    }
}
