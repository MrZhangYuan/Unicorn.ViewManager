using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
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
                        if (this._autoHideWindow.Height.IsNonreal())
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
