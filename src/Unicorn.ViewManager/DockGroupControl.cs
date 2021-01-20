using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.ViewManager.Internal;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Unicorn.ViewManager
{
    public class TabGroupControl : TabControl
    {
        static TabGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabGroupControl), new FrameworkPropertyMetadata(typeof(TabGroupControl)));
        }

        public TabGroupControl()
        {

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
    }


    public class TabGroupTabItem : TabItem
    {
        static TabGroupTabItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabGroupTabItem), new FrameworkPropertyMetadata(typeof(TabGroupTabItem)));
        }

        public TabGroupControl ParentHost
        {
            get;
            internal set;
        }

        public void UnDock()
        {
            if (this.ParentHost != null)
            {
                this.ParentHost.UnDock(this);
            }
        }
    }

    /// <summary>
    /// DockGroupControl 只允许停靠 TabGroupControl 和 DockGroupControl
    /// </summary>
    public class DockGroupControl : SplitterItemsControl
    {
        static DockGroupControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockGroupControl), new FrameworkPropertyMetadata(typeof(DockGroupControl)));
        }

        public DockGroupControl()
        {

        }


        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<DockGroupControl>())
                {
                    item.ParentHost = null;
                }
                foreach (var item in e.OldItems.OfType<TabGroupControl>())
                {
                    item.ParentHost = null;
                }
            }

            foreach (var item in this.Items)
            {
                if (item is DockGroupControl dockgroup)
                {
                    dockgroup.ParentHost = this;
                }
                else if (item is TabGroupControl tabgroup)
                {
                    tabgroup.ParentHost = this;
                }
                else
                {
                    //TODO 不允许插入其它类型的的元素
                }
            }

            if (this.Items.Count==0 
                && this.ParentHost!=null)
            {
                this.ParentHost.UnDock(this);
            }
        }


        public DockGroupControl ParentHost
        {
            get;
            internal set;
        }

        public void Dock(DockDirection direction, DependencyObject dobj)
        {
            switch (direction)
            {
                case DockDirection.Fill:
                    this.Items.Add(dobj);
                    break;
            }
        }

        public void UnDock(DependencyObject dobj)
        {
            this.Items.Remove(dobj);

            //if (this.ParentHost != null)
            //{
            //    if (this.Items.Count == 0)
            //    {
            //        this.ParentHost.UnDock(this);
            //    }
            //    else if (this.Items.Count == 1
            //        && this.ParentHost != null)
            //    {
            //        var child = this.Items[0];

            //        var parentindex = this.ParentHost.Items.IndexOf(this);

            //        if (parentindex >= 0)
            //        {             
            //            this.Items.Remove(child);
            //            this.ParentHost.Items.Insert(parentindex, child);
            //        }
            //    }
            //}
        }

        //private TabControl _innerChild = null;
        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();

        //    this._innerChild = this.OnCreateChildTabControl();

        //    var childhost = this.GetTemplateChild("") as ContentPresenter;
        //    if (childhost != null)
        //    {

        //    }
        //}

        //protected virtual TabControl OnCreateChildTabControl()
        //{
        //    return new TabGroupControl();
        //}



        //protected sealed override bool IsItemItsOwnContainerOverride(object item)
        //{
        //    return item is DockItem
        //        || item is DockGroupControl;
        //}

        //protected sealed override DependencyObject GetContainerForItemOverride()
        //{
        //    return new DockItem();
        //}
    }
}


