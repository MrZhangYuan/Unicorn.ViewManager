using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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
    //public interface IDockHost
    //{
    //    IDockHost ParentHost { get; set; }
    //    void Dock(DockDirection direction, DependencyObject child);
    //    void UnDock();
    //    void UnDock(IDockHost dobj);
    //}

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
                    //不允许插入其它类型的的元素
                    throw new NotSupportedException($"{typeof(DockGroupControl)} 只允许容纳 {typeof(DockGroupControl)}、{typeof(TabGroupControl)} 作为其子元素");
                }
            }

            if (this.Items.Count == 0
                && this.ParentHost != null)
            {
                this.ParentHost.UnDock(this);
            }
        }


        public DockGroupControl ParentHost
        {
            get;
            private set;
        }

        public void Dock(DockDirection direction, DependencyObject dobj)
        {
            switch (direction)
            {
                case DockDirection.Fill:
                    this.Items.Add(dobj);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public void UnDock(DependencyObject dobj)
        {
            this.Items.Remove(dobj);
        }
    }
}


