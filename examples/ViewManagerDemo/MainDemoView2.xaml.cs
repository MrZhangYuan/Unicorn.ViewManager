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

namespace ViewManagerDemo
{
    /// <summary>
    /// MainDemoView2.xaml 的交互逻辑
    /// </summary>
    public partial class MainDemoView2 : UserControl
    {

        private static MainDemoView2 _instance = null;
        public static MainDemoView2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainDemoView2();
                }
                return _instance;
            }
        }

        public MainDemoView2()
        {
            InitializeComponent();
        }
    }
}
