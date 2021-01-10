using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            DockManager.Instance.Init();
        }


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


        public IPopupItemContainer ActiveContainer
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
