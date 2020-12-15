using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Unicorn.ViewManager.Preferences;

namespace Unicorn.ViewManager
{
    public class ViewManager : IRichViewContainer, IPopupItemContainer
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
    }
}
