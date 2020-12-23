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
using Unicorn.ViewManager;
using ViewManagerDemo.Dialogs;

namespace ViewManagerDemo.Views
{
    /// <summary>
    /// DialogsDemoView.xaml 的交互逻辑
    /// </summary>
    public partial class DialogsDemoView
    {

        private static DialogsDemoView _instance = null;
        public static DialogsDemoView Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DialogsDemoView();
                }
                return _instance;
            }
        }

        public DialogsDemoView()
        {
            InitializeComponent();
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)e.OriginalSource).Name)
            {
                case "_showFullScreen":
                   this.Show(new FullScreenDialog());
                    break;

                case "_showNormal":
                    this.Show(new NormalDialog());
                    break;
                case "_showEventBt":
                    this.Show(new DialogWithEvent());
                    break;

                case "_showDialogAsModalBt":
                    var modalresult = this.ShowModal(new NormalDialog() { SetModalResultBtVisibility = Visibility.Visible });
                    if (modalresult != null)
                    {
                        MessageDialogBox.Show(modalresult.Result + "");
                    }
                    break;
            }
        }
    }
}
