using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public class AutoHideChannelControl : CustomItemsControl
    {
        public AutoHideRootControl ParentHost
        {
            get;
            internal set;
        }

        public object AutoHideSlideout
        {
            get
            {
                return (object)GetValue(AutoHideSlideoutProperty);
            }
            set
            {
                SetValue(AutoHideSlideoutProperty, value);
            }
        }
        public static readonly DependencyProperty AutoHideSlideoutProperty = DependencyProperty.Register("AutoHideSlideout", typeof(object), typeof(AutoHideChannelControl), new PropertyMetadata(null));


        public static Dock GetChannelDock(DependencyObject obj)
        {
            return (Dock)obj.GetValue(ChannelDockProperty);
        }

        public static void SetChannelDock(DependencyObject obj, Dock value)
        {
            obj.SetValue(ChannelDockProperty, value);
        }

        public static readonly DependencyProperty ChannelDockProperty = DependencyProperty.RegisterAttached("ChannelDock", typeof(Dock), typeof(AutoHideChannelControl), new PropertyMetadata(System.Windows.Controls.Dock.Left));

        static AutoHideChannelControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideChannelControl), new FrameworkPropertyMetadata(typeof(AutoHideChannelControl)));
        }


        public AutoHideChannelControl()
        {

        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is AutoHideChannelItem channelItem)
                    {
                        channelItem.ParentHost = null;
                    }
                }
            }

            foreach (var item in this.Items)
            {
                if (item is AutoHideChannelItem channelItem)
                {
                    AutoHideChannelControl.SetChannelDock(channelItem, AutoHideChannelControl.GetChannelDock(this));
                    channelItem.ParentHost = this;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }


        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is AutoHideChannelItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new AutoHideChannelItem();
        }

        public void Dock(AutoHideChannelItem channelitem)
        {
            if (!this.Items.Contains(channelitem))
            {
                this.Items.Add(channelitem);
            }
        }

        public void UnDock(DependencyObject item)
        {
            this.Items.Remove(item);
        }
    }
}
