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

namespace ViewManagerDemo.Dialogs
{
    /// <summary>
    /// FullScreenDialog.xaml 的交互逻辑
    /// </summary>
    public partial class FullScreenDialog
    {
        public FullScreenDialog()
        {
            InitializeComponent();
            this.Shown += FullScreenDialog_Shown;
        }

        private void FullScreenDialog_Shown(object sender, EventArgs e)
        {
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            switch (((Button)e.OriginalSource).Name)
            {
                case "_showChildDialogBt":
                    this.Show(new NormalDialog());
                    break;
            }
        }
    }
}
