using System;
using Unicorn.Utilities.Collections;

namespace Unicorn.ViewManager
{
    static class AutoHideManager
    {
        private static readonly WeakCollection<AutoHideChannelItem> _allTabViews = new WeakCollection<AutoHideChannelItem>(50);

        static AutoHideManager()
        {

        }



        public static void RegisterChannelItem(AutoHideChannelItem item)
        {

        }

        public static void Dock(DockSiteAdorner hitsite, TabGroupTabItem draggedtab)
        {
            var antohideroot = hitsite.AdornedDockTarget.FindAncestor<AutoHideRootControl>();

            if (antohideroot.DockRoot == null)
            {
                throw new InvalidOperationException();
            }

            switch (hitsite.DockDirection)
            {
                case DockDirection.Fill:
                    throw new NotSupportedException();

                case DockDirection.Left:
                case DockDirection.Right: 
                case DockDirection.Top:
                case DockDirection.Bottom:
                    {
                        antohideroot.DockRoot.Dock(hitsite.DockDirection, draggedtab);
                    }
                    break;
            }
        }
    }
}
