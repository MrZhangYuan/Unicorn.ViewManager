using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.Utilities;

namespace Unicorn.ViewManager
{
    [TemplatePart(Name = PART_RICHVIEWCONTROL, Type = typeof(ContentPresenter))]
    public class RichViewWindow : CustomChromeWindow, IPopupItemContainer
    {
        private const string PART_RICHVIEWCONTROL = "PART_RICHVIEWCONTROL";

        private readonly RichViewControl _richViewControl = new RichViewControl();

        public RichViewControl RichViewControl
        {
            get
            {
                return this._richViewControl;
            }
        }

        public PopupItem TopItem => this._richViewControl.TopItem;

        IPopupItemContainer IPopupItemContainer.Parent => null;

        public IEnumerable<PopupItem> Children => this._richViewControl.Children;

        public event ViewStackChangedEventHandler ViewStackChanged
        {
            add
            {
                this.RichViewControl.ViewStackChanged += value;
            }
            remove
            {
                this.RichViewControl.ViewStackChanged -= value;
            }
        }

        public object Header
        {
            get
            {
                return (object)GetValue(HeaderProperty);
            }
            set
            {
                SetValue(HeaderProperty, value);
            }
        }
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(object), typeof(RichViewWindow), new PropertyMetadata(null));

        static RichViewWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichViewWindow), new FrameworkPropertyMetadata(typeof(RichViewWindow)));
        }

        public RichViewWindow()
        {
            this.IsVisibleChanged += RichViewWindow_IsVisibleChanged;
            WindowManager.RegisterWindow(this);
        }

        private void RichViewWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                DockManager.Instance.RegisterDockSite(this);
            }
            else
            {
                DockManager.Instance.UnregisterDockSite(this);
            }
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var persenter = this.GetTemplateChild(PART_RICHVIEWCONTROL) as ContentPresenter;
            if (persenter != null)
            {
                persenter.Content = this._richViewControl;
            }
        }

        public void Close(PopupItem item)
        {
            this._richViewControl.Close(item);
        }

        bool IPopupItemContainer.Close()
        {
            return this._richViewControl.Close();
        }

        public void Show(PopupItem item)
        {
            this._richViewControl.Show(item);
        }

        public ModalResult ShowModal(PopupItem item)
        {
            return this._richViewControl.ShowModal(item);
        }
    }
}
