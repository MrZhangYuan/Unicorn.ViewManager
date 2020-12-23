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
using System.Windows.Shapes;
using Unicorn.ViewManager;

namespace ViewManagerDemo
{
    /// <summary>
    /// StandardWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StandardWindow : IPopupItemContainer
    {
        private readonly RichViewControl _richViewControl = new RichViewControl();

        public PopupItem TopItem => this._richViewControl.TopItem;
        IPopupItemContainer IPopupItemContainer.Parent => null;
        public IEnumerable<PopupItem> Children => this._richViewControl.Children;

        public StandardWindow()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var persenter = this.GetTemplateChild("PART_RICHVIEWCONTROL") as ContentPresenter;
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


        public static void ShowStandard(PopupItem topitem)
        {
            topitem.Closed += StandardView_Closed;
            topitem.Close();
        }
        private static void StandardView_Closed(object sender, EventArgs e)
        {
            PopupItem popupItem = sender as PopupItem;
            popupItem.Closed -= StandardView_Closed;

            StandardWindow window = new StandardWindow();
            window.Show();
            window.Activate();
            window.Show(popupItem);
        }
    }
}
