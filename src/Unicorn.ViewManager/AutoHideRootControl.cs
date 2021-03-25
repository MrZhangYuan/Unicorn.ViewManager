using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    public class AutoHideRootControl : CustomItemsControl
    {
        private AutoHideChannelControl[] _autoHideChannels = new AutoHideChannelControl[4];
        private AutoHideChannelControl EnsureInitlizeChannels(AutoHideChannelControl channel, Dock dock)
        {
            var index = (int)dock;
            if (this._autoHideChannels[index] == null)
            {
                if (channel == null)
                {
                    channel = new AutoHideChannelControl()
                    {
                        ParentHost = this
                    };
                }

                this._autoHideChannels[index] = channel;
                this._autoHideChannels[index].SetValue(AutoHideChannelControl.ChannelDockProperty, dock);
                this._autoHideChannels[index].ItemsChanged += AutoHideRootControl_ItemsChanged;
                this.Items.Add(this._autoHideChannels[index]);
            }

            return this._autoHideChannels[index];
        }

        private void AutoHideRootControl_ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is AutoHideChannelControl autoHideChannel)
            {
                autoHideChannel.Visibility = autoHideChannel.Items.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }



        private DockRootControl EnsureInitlizeDockRoot(DockRootControl dockroot)
        {
            if (this._dockRoot == null)
            {
                this._dockRoot = dockroot != null ? dockroot : new DockRootControl();
            }

            if (!this.Items.Contains(this._dockRoot))
            {
                this.Items.Add(this._dockRoot);
            }

            return this._dockRoot;
        }

        private DockRootControl _dockRoot = null;
        public DockRootControl DockRoot
        {
            get => EnsureInitlizeDockRoot(null);
        }

        public AutoHideChannelControl AutoHideChannelLeft
        {
            get => EnsureInitlizeChannels(null, Dock.Left);
        }

        public AutoHideChannelControl AutoHideChannelTop
        {
            get => EnsureInitlizeChannels(null, Dock.Top);
        }

        public AutoHideChannelControl AutoHideChannelRight
        {
            get => EnsureInitlizeChannels(null, Dock.Right);
        }

        public AutoHideChannelControl AutoHideChannelBottom
        {
            get => EnsureInitlizeChannels(null, Dock.Bottom);
        }


        static AutoHideRootControl()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(AutoHideRootControl), 
                new FrameworkPropertyMetadata(typeof(AutoHideRootControl)));

            CommandManager.RegisterClassCommandBinding(
                typeof(AutoHideRootControl),
                new CommandBinding(
                    ViewCommands.HideToolTabToAutoHide,
                    new ExecutedRoutedEventHandler(AutoHideRootControl.OnHideToolTabToAutoHide), 
                    new CanExecuteRoutedEventHandler(AutoHideRootControl.OnCanHideToolTabToAutoHide)));

            CommandManager.RegisterClassCommandBinding(
                typeof(AutoHideRootControl),
                new CommandBinding(
                    ViewCommands.UnHideAutoHideToToolTab,
                    new ExecutedRoutedEventHandler(AutoHideRootControl.OnUnHideAutoHideToToolTab), 
                    new CanExecuteRoutedEventHandler(AutoHideRootControl.OnCanUnHideAutoHideToToolTab)));
        }

        private static void OnCanUnHideAutoHideToToolTab(object sender, CanExecuteRoutedEventArgs e)
        {
            AutoHideRootControl autoHideRoot = (AutoHideRootControl)sender;
            if (e.Parameter is TabGroupTabItem item)
            {
                //switch ((Dock)item.GetValue(AutoHideChannelControl.ChannelDockProperty))
                //{
                //    case Dock.Top:
                //        break;

                //    case Dock.Right:
                //        break;

                //    case Dock.Bottom:
                //        break;

                //    case Dock.Left:
                //    default:

                //        break;
                //}
                //e.CanExecute = autoHideRoot.EnsureInitlizeChannels(null, (Dock)item.GetValue(AutoHideChannelControl.ChannelDockProperty)).Items.Contains(item);
            }

            e.CanExecute = true;

            e.Handled = true;
        }

        private static void OnUnHideAutoHideToToolTab(object sender, ExecutedRoutedEventArgs e)
        {
            AutoHideRootControl autoHideRoot = (AutoHideRootControl)sender;
            if (e.Parameter is TabGroupTabItem item)
            {
                item.UnDock();
                switch ((Dock)item.GetValue(AutoHideChannelControl.ChannelDockProperty))
                {
                    case Dock.Top:
                        autoHideRoot.DockRoot.Dock(DockDirection.Top, item);
                        break;

                    case Dock.Right:
                        autoHideRoot.DockRoot.Dock(DockDirection.Right, item);
                        break;

                    case Dock.Bottom:
                        autoHideRoot.DockRoot.Dock(DockDirection.Bottom, item);
                        break;

                    case Dock.Left:
                    default:
                        autoHideRoot.DockRoot.Dock(DockDirection.Left, item);
                        break;
                }
            }
        }

        private static void OnCanHideToolTabToAutoHide(object sender, CanExecuteRoutedEventArgs e)
        {
            AutoHideRootControl autoHideRoot = (AutoHideRootControl)sender;
            e.CanExecute = e.Parameter is TabGroupTabItem item;
            e.Handled = true;
        }

        private static void OnHideToolTabToAutoHide(object sender, ExecutedRoutedEventArgs e)
        {
            AutoHideRootControl autoHideRoot = (AutoHideRootControl)sender;
            if (e.Parameter is TabGroupTabItem item)
            {
                var autoHideChannel = autoHideRoot.EnsureInitlizeChannels(
                    null,
                    (Dock)item.GetValue(AutoHideChannelControl.ChannelDockProperty));

                AutoHideChannelItem channelItem = new AutoHideChannelItem()
                {
                    Title = item.Title
                };
                channelItem.Dock(item);
                autoHideChannel.Dock(channelItem);

                if (!autoHideRoot.Items.Contains(autoHideChannel))
                {
                    autoHideRoot.Items.Add(autoHideChannel);
                }
            }
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            UIElement uIElement = (UIElement)element;

            if (element is AutoHideChannelControl channelControl)
            {
                //已经初始化了对应位置的AutoHideChannelControl，不允许再次赋予不同的对象
                var index = (int)AutoHideChannelControl.GetChannelDock(channelControl);
                if (_autoHideChannels[index] != null
                    && !object.ReferenceEquals(_autoHideChannels[index], channelControl))
                {
                    throw new InvalidOperationException();
                }

                Panel.SetZIndex(uIElement, 1);

                switch (AutoHideChannelControl.GetChannelDock(channelControl))
                {
                    case Dock.Left:
                        Grid.SetColumn(uIElement, 0);
                        Grid.SetRow(uIElement, 1);
                        break;
                    case Dock.Right:
                        Grid.SetColumn(uIElement, 2);
                        Grid.SetRow(uIElement, 1);
                        break;
                    case Dock.Top:
                        Grid.SetColumn(uIElement, 1);
                        Grid.SetRow(uIElement, 0);
                        break;
                    case Dock.Bottom:
                        Grid.SetColumn(uIElement, 1);
                        Grid.SetRow(uIElement, 2);
                        break;
                }
            }
            else
            {
                if (element is DockRootControl dockroot)
                {
                    this.EnsureInitlizeDockRoot(dockroot);
                }

                Panel.SetZIndex(uIElement, 0);
                Grid.SetColumn(uIElement, 1);
                Grid.SetRow(uIElement, 1);
            }
        }
    }


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

    public class AutoHideChannelItem : Button
    {
        public object Title
        {
            get
            {
                return (object)GetValue(TitleProperty);
            }
            set
            {
                SetValue(TitleProperty, value);
            }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(object), typeof(AutoHideChannelItem), new PropertyMetadata(null));

        public AutoHideChannelControl ParentHost
        {
            get;
            internal set;
        }

        private readonly ToolTabGroupControl _toolTabGroupControl = new ToolTabGroupControl();
        private readonly AutoHideWindow _autoHideWindow = new AutoHideWindow()
        {
            MinHeight = 20,
            MinWidth = 20
        };
        static AutoHideChannelItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideChannelItem), new FrameworkPropertyMetadata(typeof(AutoHideChannelItem)));
        }

        public AutoHideChannelItem()
        {
            this._toolTabGroupControl.ItemsChanged += _toolTabGroupControl_ItemsChanged;
            this._autoHideWindow.Content = this._toolTabGroupControl;

            AutoHideManager.RegisterChannelItem(this);
            this.Click += AutoHideChannelItem_Click;
        }

        private void _toolTabGroupControl_ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this._toolTabGroupControl.Items.Count == 0)
            {
                this.UnDock();
            }
        }

        private void AutoHideChannelItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.ParentHost.AutoHideSlideout == this._autoHideWindow)
            {
                this.ParentHost.AutoHideSlideout = null;
            }
            else
            {
                var parentdock = AutoHideChannelControl.GetChannelDock(this.ParentHost);

                switch (parentdock)
                {
                    case System.Windows.Controls.Dock.Left:
                    case System.Windows.Controls.Dock.Right:
                        if (this._autoHideWindow.Width.IsNonreal())
                        {
                            this._autoHideWindow.Width = 300;
                        }
                        break;

                    case System.Windows.Controls.Dock.Top:
                    case System.Windows.Controls.Dock.Bottom:
                        if (this._autoHideWindow.Width.IsNonreal())
                        {
                            this._autoHideWindow.Height = 300;
                        }
                        break;
                }

                AutoHideChannelControl.SetChannelDock(this._autoHideWindow, parentdock);

                this.ParentHost.AutoHideSlideout = this._autoHideWindow;
            }
        }

        public void Dock(TabGroupTabItem tabItem)
        {
            tabItem.UnDock();
            this._toolTabGroupControl.Dock(tabItem);
        }

        public void UnDock()
        {
            if (this.ParentHost != null)
            {
                if (this.ParentHost.AutoHideSlideout == this._autoHideWindow)
                {
                    this.ParentHost.AutoHideSlideout = null;
                }
                this.ParentHost.UnDock(this);
            }
        }
    }
}
