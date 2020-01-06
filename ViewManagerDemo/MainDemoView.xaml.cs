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
    /// MainDemoView.xaml 的交互逻辑
    /// </summary>
    public partial class MainDemoView : UserControl
    {

        private static MainDemoView _instance = null;
        public static MainDemoView Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainDemoView();
                }
                return _instance;
            }
        }

        public MainDemoView()
        {
            InitializeComponent();
        }
    }
}
