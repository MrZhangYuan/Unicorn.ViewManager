using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

namespace Unicorn.ViewManager
{
    public abstract class TabGroupControl : CustomTabControl
    {
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
            }

            if (this.Items.Count == 1)
            {
                this.SelectedItem = this.Items[0];
            }
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

        public void Dock(TabGroupTabItem tabitem)
        {
            if (!this.Items.Contains(tabitem))
            {
                this.Items.Add(tabitem);
            }
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

        public abstract TabGroupControl CreateTabGroup();
    }
}


