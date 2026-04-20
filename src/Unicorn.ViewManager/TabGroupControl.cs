using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Unicorn.ViewManager
{
    public abstract class TabGroupControl : CustomTabControl
    {
        private readonly LinkedList<TabGroupTabItem> _selectionPriorityQueue = new LinkedList<TabGroupTabItem>();
        private bool _suspendSelectionRestore;

        static TabGroupControl()
        {
            CommandManager.RegisterClassCommandBinding(typeof(TabGroupControl), new CommandBinding(ViewCommands.CloseToolTab, new ExecutedRoutedEventHandler(TabGroupControl.OnCloseToolTab), new CanExecuteRoutedEventHandler(TabGroupControl.OnCanCloseToolTab)));
            CommandManager.RegisterClassCommandBinding(typeof(TabGroupControl), new CommandBinding(ViewCommands.CloseViewTab, new ExecutedRoutedEventHandler(TabGroupControl.OnCloseViewTab), new CanExecuteRoutedEventHandler(TabGroupControl.OnCanCloseViewTab)));
        }

        private static void OnCanCloseViewTab(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is TabGroupControl tabgroup)
            {
                e.CanExecute = e.Parameter is TabGroupTabItem item && tabgroup.Items.Contains(item);
                e.Handled = true;
            }
        }

        private static void OnCloseViewTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is TabGroupControl tabgroup)
            {
                tabgroup.UnDock(e.Parameter as TabGroupTabItem);
                e.Handled = true;
            }
        }

        private static void OnCanCloseToolTab(object sender, CanExecuteRoutedEventArgs e)
        {
            if (sender is TabGroupControl tabgroup)
            {
                e.CanExecute = e.Parameter is TabGroupTabItem item && tabgroup.Items.Contains(item);
                e.Handled = true;
            }
        }

        private static void OnCloseToolTab(object sender, ExecutedRoutedEventArgs e)
        {
            if (sender is TabGroupControl tabgroup)
            {
                tabgroup.UnDock(e.Parameter as TabGroupTabItem);
                e.Handled = true;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.OldItems != null)
            {
                foreach (TabGroupTabItem item in e.OldItems)
                {
                    item.ParentHost = null;
                    this.RemoveFromSelectionPriorityQueue(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (TabGroupTabItem item in e.NewItems)
                {
                    this.EnqueueSelectionPriority(item);
                }
            }

            foreach (TabGroupTabItem item in this.Items)
            {
                item.ParentHost = this;
            }

            if (this.Items.Count == 0
                && this.ParentHost != null)
            {
                this.ParentHost.UnDock(this);
                return;
            }

            if (this.Items.Count == 1)
            {
                this.SelectedItem = this.Items[0];
                return;
            }

            if (!this._suspendSelectionRestore
                && this.Items.Count > 1
                && !(this.SelectedItem is TabGroupTabItem selectedTab && this.Items.Contains(selectedTab)))
            {
                this.RestoreSelectedItemFromPriorityQueue();
            }
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            if (this.SelectedItem is TabGroupTabItem selectedTab)
            {
                this.MoveSelectionPriorityToFront(selectedTab);
            }

            this.NotifyCurrentSelection();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            this.NotifyCurrentSelection();
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            this.NotifyCurrentSelection();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabGroupTabItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabGroupTabItem();
        }

        public DockGroupControl ParentHost
        {
            get;
            internal set;
        }

        private void NotifyCurrentSelection()
        {
            if (this.SelectedItem is TabGroupTabItem selectedTab)
            {
                ViewManager.Instance.NotifyTabSelection(this, selectedTab);
            }
        }

        public void Dock(TabGroupTabItem tabitem)
        {
            this.Dock(tabitem, this.Items.Count);
        }

        public void Dock(TabGroupTabItem tabitem, int index)
        {
            if (tabitem == null)
            {
                return;
            }

            if (index < 0)
            {
                index = 0;
            }

            if (index > this.Items.Count)
            {
                index = this.Items.Count;
            }

            if (this.Items.Contains(tabitem))
            {
                this._suspendSelectionRestore = true;
                try
                {
                    this.Items.Remove(tabitem);
                }
                finally
                {
                    this._suspendSelectionRestore = false;
                }

                if (index > this.Items.Count)
                {
                    index = this.Items.Count;
                }
            }

            this.Items.Insert(index, tabitem);
            this.SelectedItem = tabitem;
        }

        public void UnDock()
        {
            if (this.ParentHost != null)
            {
                this.ParentHost.UnDock(this);
            }
        }

        public void UnDock(DependencyObject dobj)
        {
            this.Items.Remove(dobj);
        }

        private void RestoreSelectedItemFromPriorityQueue()
        {
            TabGroupTabItem fallbackTab = this.GetHighestPriorityExistingTab();
            if (fallbackTab != null)
            {
                this.SelectedItem = fallbackTab;
                return;
            }

            if (this.Items.Count > 0)
            {
                this.SelectedItem = this.Items[0];
            }
        }

        private TabGroupTabItem GetHighestPriorityExistingTab()
        {
            LinkedListNode<TabGroupTabItem> node = this._selectionPriorityQueue.First;
            while (node != null)
            {
                LinkedListNode<TabGroupTabItem> next = node.Next;
                TabGroupTabItem candidate = node.Value;
                if (candidate != null
                    && ReferenceEquals(candidate.ParentHost, this)
                    && this.Items.Contains(candidate))
                {
                    return candidate;
                }

                this._selectionPriorityQueue.Remove(node);
                node = next;
            }

            return null;
        }

        private void EnqueueSelectionPriority(TabGroupTabItem item)
        {
            if (item == null)
            {
                return;
            }

            this.RemoveFromSelectionPriorityQueue(item);
            this._selectionPriorityQueue.AddLast(item);
        }

        private void MoveSelectionPriorityToFront(TabGroupTabItem item)
        {
            if (item == null)
            {
                return;
            }

            this.RemoveFromSelectionPriorityQueue(item);
            this._selectionPriorityQueue.AddFirst(item);
        }

        private void RemoveFromSelectionPriorityQueue(TabGroupTabItem item)
        {
            if (item == null)
            {
                return;
            }

            LinkedListNode<TabGroupTabItem> node = this._selectionPriorityQueue.First;
            while (node != null)
            {
                LinkedListNode<TabGroupTabItem> next = node.Next;
                if (ReferenceEquals(node.Value, item))
                {
                    this._selectionPriorityQueue.Remove(node);
                }

                node = next;
            }
        }

        public abstract TabGroupControl CreateTabGroup();
    }
}
