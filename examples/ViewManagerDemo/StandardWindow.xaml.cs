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
    public partial class StandardWindow : RichViewWindow
    {
        public StandardWindow()
        {
            InitializeComponent();
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
