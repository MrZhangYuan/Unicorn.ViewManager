using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ViewManagerDemo.Views
{
    /// <summary>
    /// ReadMe.xaml 的交互逻辑
    /// </summary>
    public partial class ReadMe : UserControl
    {

        private static ReadMe _instance = null;
        public static ReadMe Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ReadMe();
                }
                return _instance;
            }
        }

        public ReadMe()
        {
            InitializeComponent();
        }

        private void TextBlock_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Hyperlink hyperlink 
                && hyperlink.NavigateUri!=null 
                && !string.IsNullOrEmpty(hyperlink.NavigateUri.AbsoluteUri))
            {
                //Process.Start(hyperlink.NavigateUri.AbsoluteUri);
            }
        }
    }
}
