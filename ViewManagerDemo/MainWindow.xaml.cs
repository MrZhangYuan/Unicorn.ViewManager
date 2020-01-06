using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.Utilities.Commands;
using Unicorn.ViewManager;
using ViewManagerDemo.Dialogs;
using ViewManagerDemo.Flyouts;

namespace ViewManagerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private static MainWindow _instance = null;
        public static MainWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainWindow();
                }
                return _instance;
            }
        }

        static MainWindow()
        {
            DefaultUICommandManager.Instance.CommandCanExecuteAction = CommandCanExecuteAction;
            DefaultUICommandManager.Instance.CommandExecuteAction = CommandExecuteAction;
        }

        private CommandListWindow _cmdListWindow = new CommandListWindow();
        public MainWindow()
        {
            InitializeComponent();
            ViewManager.Instance.InitializeRichView(this);
            ViewManager.Instance.ViewPreferences.UsePopupViewAnimations = true;
            ViewManager.Instance.MainRichView.SwitchView(MainDemoView.Instance);

            this.IsVisibleChanged += (sender, e) =>
            {
                if (this.IsVisible)
                {
                    this._cmdListWindow.Show();
                }
            };
            this.SizeChanged += (sender, e) =>
            {
                this.RefreshCmdWindowLocation();
            };
            this.StateChanged += (sender, e) =>
            {
                this.RefreshCmdWindowLocation();
            };
            this.LocationChanged += (sender, e) =>
            {
                this.RefreshCmdWindowLocation();
            };
        }

        public void RefreshCmdWindowLocation()
        {
            this._cmdListWindow.Owner = this;
            this._cmdListWindow.Height = this.ActualHeight;
            this._cmdListWindow.Left = this.Left + this.ActualWidth;
            this._cmdListWindow.Top = this.Top;
        }

        private static bool CommandCanExecuteAction(string cmdkey, UICommandParameter<string> parameter)
        {
            return true;
        }

        private static void CommandExecuteAction(string cmdkey, UICommandParameter<string> parameter)
        {
            switch (cmdkey)
            {
                case "SwitchMainView1":
                    ViewManager.Instance.MainRichView.SwitchView(MainDemoView.Instance);
                    break;

                case "SwitchMainView2":
                    ViewManager.Instance.MainRichView.SwitchView(MainDemoView2.Instance);
                    break;

                case "ShowFlyoutLeft":
                    ViewManager.Instance.MainRichView.Show(new FlyoutLocationDemo
                    {
                        FlyoutLocation = FlyoutLocation.Left,
                        Width = 250
                    });
                    break;

                case "ShowFlyoutTop":
                    ViewManager.Instance.MainRichView.Show(new FlyoutLocationDemo
                    {
                        FlyoutLocation = FlyoutLocation.Top,
                        Height = 250
                    });
                    break;

                case "ShowFlyoutRight":
                    ViewManager.Instance.MainRichView.Show(new FlyoutLocationDemo
                    {
                        FlyoutLocation = FlyoutLocation.Right,
                        Width = 250
                    });
                    break;

                case "ShowFlyoutBottom":
                    ViewManager.Instance.MainRichView.Show(new FlyoutLocationDemo
                    {
                        FlyoutLocation = FlyoutLocation.Bottom,
                        Height = 250
                    });
                    break;


                case "ShowDialogFullScreen":
                    ViewManager.Instance.MainRichView.Show(new FullScreenDialog());
                    break;

                case "ShowDialogNormal":
                    ViewManager.Instance.MainRichView.Show(new NormalDialog());
                    break;

                //当Dialog以模态显示时，其同一个显示堆栈上只可以再堆叠MessageDialogBox，其余的任何可弹出组件不可显示
                case "ShowDialogAsModal":
                    {
                        ModalResult result = ViewManager.Instance.MainRichView.ShowModal(new NormalDialog());
                        if (result != null)
                        {
                            MessageDialogBox.Show(result.Result + "");
                        }
                    }
                    break;

                case "ShowMessageDialogBox":
                    {
                        var msresult = MessageDialogBox.Show($"是、否、取消MessageDialogBox，根据需要返回结果，请继续...{Environment.NewLine}[是]：是{Environment.NewLine}[否]：否{Environment.NewLine}[取消]：取消", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if (msresult == MessageBoxResult.Yes)
                        {
                            MessageDialogBox.Show("你点击了 [是]");
                        }
                        else if (msresult == MessageBoxResult.No)
                        {
                            MessageDialogBox.Show("你点击了 [否]");
                        }
                        else if (msresult == MessageBoxResult.Cancel)
                        {
                            MessageDialogBox.Show("你点击了 [取消]，或者直接关闭了");
                        }
                        else
                        {
                            MessageDialogBox.Show("不应该返回此结果");
                        }
                    }
                    break;

                case "Exit":
                    MainWindow.Instance.Close();
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            var msresult = MessageDialogBox.Show($"是否确认关闭？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msresult == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
