using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unicorn.Utilities.Collections;
using Unicorn.ViewManager.Preferences;

namespace Unicorn.ViewManager
{
    public class ViewManager
    {
        public static ViewManager Instance
        {
            get;
            private set;
        }

        static ViewManager()
        {
            Instance = new ViewManager();
        }

        private ViewManager()
        {

        }


        private readonly WeakCollection<TabGroupTabItem> _allTabViews = new WeakCollection<TabGroupTabItem>(50);
        private readonly Dictionary<Window, TabGroupTabItem> _selectedTabsByWindow = new Dictionary<Window, TabGroupTabItem>();
        private readonly HashSet<Window> _trackedWindows = new HashSet<Window>();
        private TabGroupTabItem _activeTabView;


        private RichViewControl _richViewControl = null;
        public RichViewControl MainRichView
        {
            get => _richViewControl ?? (_richViewControl = new RichViewControl());
        }

        public ViewPreferences ViewPreferences
        {
            get => ViewPreferences.Instance;
        }

        public ContentControl HostContentControl
        {
            get;
            private set;
        }

        public void InitializeRichView(ContentControl contentControl)
        {
            if (contentControl == null)
                throw new ArgumentNullException(nameof(contentControl));

            this.HostContentControl = contentControl;
            contentControl.Content = this.MainRichView;
        }


        public ModalResult ShowModal(PopupItem item)
        {
            return this.MainRichView.ShowModal(item);
        }

        public void Show(PopupItem item)
        {
            this.MainRichView.Show(item);
        }

        public void Close(PopupItem item)
        {
            this.MainRichView.Close(item);
        }

        public void ShowView(object item)
        {
            this.MainRichView.ShowView(item);
        }

        public void CloseView(object item)
        {
            this.MainRichView.CloseView(item);
        }

        public void SwitchView(object item)
        {
            this.MainRichView.SwitchView(item);
        }

        public bool Close()
        {
            return this.MainRichView.Close();
        }

        internal void RegisterTabView(TabGroupTabItem tabitem)
        {
            this._allTabViews.Add(tabitem);
            tabitem.Loaded += OnTabItemLoaded;
            tabitem.Unloaded += OnTabItemUnloaded;
        }

        private void OnTabItemLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is TabGroupTabItem tabitem)
            {
                Window window = Window.GetWindow(tabitem);
                this.RegisterWindow(window);

                if (tabitem.IsSelected
                    && tabitem.ParentHost != null)
                {
                    this.NotifyTabSelection(tabitem.ParentHost, tabitem);
                }
            }
        }

        private void OnTabItemUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is TabGroupTabItem tabitem)
            {
                if (ReferenceEquals(this._activeTabView, tabitem))
                {
                    tabitem.IsActive = false;
                    this._activeTabView = null;
                }

                Window window = Window.GetWindow(tabitem);
                if (window != null
                    && this._selectedTabsByWindow.TryGetValue(window, out TabGroupTabItem selectedTab)
                    && ReferenceEquals(selectedTab, tabitem))
                {
                    this._selectedTabsByWindow.Remove(window);
                }
            }
        }

        internal void NotifyTabSelection(TabGroupControl tabgroup, TabGroupTabItem selectedTab)
        {
            if (tabgroup == null
                || selectedTab == null)
            {
                return;
            }

            Window window = Window.GetWindow(tabgroup);
            this.RegisterWindow(window);

            if (window != null)
            {
                this._selectedTabsByWindow[window] = selectedTab;

                if (window.IsActive)
                {
                    this.SetActiveTab(selectedTab);
                }
            }
            else
            {
                this.SetActiveTab(selectedTab);
            }
        }

        internal void SetActiveTab(TabGroupTabItem tabitem)
        {
            if (tabitem == null)
            {
                return;
            }

            if (ReferenceEquals(this._activeTabView, tabitem))
            {
                if (!tabitem.IsActive)
                {
                    tabitem.IsActive = true;
                }
                return;
            }

            if (this._activeTabView != null)
            {
                this._activeTabView.IsActive = false;
            }

            this._activeTabView = tabitem;
            this._activeTabView.IsActive = true;
        }

        private void RegisterWindow(Window window)
        {
            if (window == null
                || this._trackedWindows.Contains(window))
            {
                return;
            }

            this._trackedWindows.Add(window);
            window.Activated += OnTrackedWindowActivated;
            window.Closed += OnTrackedWindowClosed;
        }

        private void OnTrackedWindowActivated(object sender, EventArgs e)
        {
            if (sender is Window window
                && this._selectedTabsByWindow.TryGetValue(window, out TabGroupTabItem selectedTab)
                && selectedTab != null)
            {
                this.SetActiveTab(selectedTab);
            }
        }

        private void OnTrackedWindowClosed(object sender, EventArgs e)
        {
            if (sender is Window window)
            {
                window.Activated -= OnTrackedWindowActivated;
                window.Closed -= OnTrackedWindowClosed;
                this._trackedWindows.Remove(window);

                if (this._selectedTabsByWindow.TryGetValue(window, out TabGroupTabItem selectedTab))
                {
                    this._selectedTabsByWindow.Remove(window);

                    if (ReferenceEquals(this._activeTabView, selectedTab))
                    {
                        this._activeTabView.IsActive = false;
                        this._activeTabView = null;
                    }
                }
            }
        }

        public IPopupItemContainer ActivePopupContainer
        {
            get
            {
                IPopupItemContainer activecontainer = null;

                UIElement element = Keyboard.FocusedElement as UIElement;
                if (element != null
                    && Window.GetWindow(element) is Window topwindow
                    && topwindow is IPopupItemContainer topcontainer)
                {
                    activecontainer = topcontainer;
                }

                if (activecontainer == null)
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.IsActive)
                        {
                            if (window is IPopupItemContainer container)
                            {
                                activecontainer = container;
                                break;
                            }
                        }
                    }
                }

                if (activecontainer == null)
                {
                    if (Application.Current.MainWindow is IPopupItemContainer main)
                    {
                        activecontainer = main;
                    }
                }

                if (activecontainer == null)
                {
                    activecontainer = this.MainRichView;
                }
                return activecontainer;
            }
        }

    }
}
