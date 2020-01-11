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
using ViewManagerDemo.Flyouts;

namespace ViewManagerDemo.Views
{
    /// <summary>
    /// FlyoutsDemoView.xaml 的交互逻辑
    /// </summary>
    public partial class FlyoutsDemoView : UserControl
    {

        private static FlyoutsDemoView _instance = null;
        public static FlyoutsDemoView Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new FlyoutsDemoView();
                }
                return _instance;
            }
        }

        public FlyoutsDemoView()
        {
            InitializeComponent();
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {
            FlyoutLocationDemo flyout = new FlyoutLocationDemo();

            switch (((Button)e.OriginalSource).Name)
            {
                case "_showFlyoutLeft":
                    flyout.Width = 250;
                    flyout.FlyoutLocation = FlyoutLocation.Left;
                    break;

                case "_showFlyoutTop":
                    flyout.Height = 250;
                    flyout.FlyoutLocation = FlyoutLocation.Top;
                    break;

                case "_showFlyoutRight":
                    flyout.Width = 250;
                    flyout.FlyoutLocation = FlyoutLocation.Right;
                    break;

                case "_showFlyoutBottom":
                    flyout.Height = 250;
                    flyout.FlyoutLocation = FlyoutLocation.Bottom;
                    break;
            }

            ViewManager.Instance.Show(flyout);
        }
    }
}
