using System;
using System.Collections.Generic;

namespace Unicorn.ViewManager
{
    public enum ViewStackAction
    {
        Add = 0,
        Remove = 1,
        Replace = 2,
        Move = 3,
        Reset = 4
    }

    public delegate void ViewStackChangedEventHandler(object sender, ViewStackChangedEventArgs e);

    public class ViewStackChangedEventArgs : EventArgs
    {
        public ViewStackAction ViewStackAction { get; }
        public IList<PopupItem> NewItems { get; }
        public int NewStartingIndex { get; }
        public IList<PopupItem> OldItems { get; }
        public int OldStartingIndex { get; }

        public ViewStackChangedEventArgs(ViewStackAction viewStackAction,
            IList<PopupItem> newItems,
            int newStartingIndex,
            IList<PopupItem> oldItems,
            int oldStartingIndex)
        {
            ViewStackAction = viewStackAction;
            NewItems = newItems;
            NewStartingIndex = newStartingIndex;
            OldItems = oldItems;
            OldStartingIndex = oldStartingIndex;
        }
    }
}
