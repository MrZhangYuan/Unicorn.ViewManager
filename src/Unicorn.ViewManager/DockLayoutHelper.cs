using System.Windows;
using System.Windows.Controls;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    internal static class DockLayoutHelper
    {
        internal static double GetPreferredDockLength(TabGroupControl sourceHost, DockDirection dockDirection)
        {
            if (sourceHost == null)
            {
                return double.NaN;
            }

            switch (dockDirection)
            {
                case DockDirection.Left:
                case DockDirection.Right:
                    return sourceHost.ActualWidth > 0.0 ? sourceHost.ActualWidth : double.NaN;

                case DockDirection.Top:
                case DockDirection.Bottom:
                    return sourceHost.ActualHeight > 0.0 ? sourceHost.ActualHeight : double.NaN;

                default:
                    return double.NaN;
            }
        }

        internal static void ApplyPreferredDockLength(DockGroupControl dockGroup, UIElement newItem, Orientation orientation, double preferredLength)
        {
            if (dockGroup == null
                || newItem == null
                || preferredLength.IsNonreal()
                || preferredLength <= 0.0)
            {
                return;
            }

            SnapshotCurrentSplitterLengths(dockGroup, orientation);
            SplitterPanel.SetSplitterLength(newItem, new SplitterLength(preferredLength));
        }

        private static void SnapshotCurrentSplitterLengths(DockGroupControl dockGroup, Orientation orientation)
        {
            foreach (object item in dockGroup.Items)
            {
                if (item is UIElement element)
                {
                    double currentLength = GetCurrentSplitterLength(element, orientation);
                    if (!currentLength.IsNonreal()
                        && currentLength > 0.0)
                    {
                        SplitterPanel.SetSplitterLength(element, new SplitterLength(currentLength));
                    }
                }
            }
        }

        internal static double GetCurrentSplitterLength(UIElement element, Orientation orientation)
        {
            double splitterLength = SplitterPanel.GetActualSplitterLength(element);
            if (!splitterLength.IsNonreal()
                && splitterLength > 0.0)
            {
                return splitterLength;
            }

            if (element is FrameworkElement frameworkElement)
            {
                return orientation == Orientation.Horizontal
                    ? frameworkElement.ActualWidth
                    : frameworkElement.ActualHeight;
            }

            return double.NaN;
        }
    }
}
