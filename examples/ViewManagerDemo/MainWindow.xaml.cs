using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using ViewManagerDemo.Views;

namespace ViewManagerDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance
        {
            get;
            private set;
        }

        static MainWindow()
        {
            DefaultUICommandManager.Instance.CommandCanExecuteAction = CommandCanExecuteAction;
            DefaultUICommandManager.Instance.CommandExecuteAction = CommandExecuteAction;
        }

        private CommandListWindow _cmdListWindow = new CommandListWindow();
        public MainWindow()
        {
            Instance = this;

            InitializeComponent();
            ViewManager.Instance.InitializeRichView(this);
            ViewManager.Instance.ViewPreferences.UsePopupViewAnimations = true;
            ViewManager.Instance.MainRichView.SwitchView(ReadMe.Instance);

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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape)
            {
                ViewManager.Instance.Close();
                e.Handled = true;
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.W:
                        var topitem = ViewManager.Instance.ActiveContainer.TopItem;
                        StandardWindow.ShowStandard(topitem);
                        break;
                }
            }
        }

        public void RefreshCmdWindowLocation()
        {
            this._cmdListWindow.Owner = this;
            this._cmdListWindow.Height = this.ActualHeight;
            this._cmdListWindow.Left = this.Left + this.ActualWidth + 1;
            this._cmdListWindow.Top = this.Top;
        }

        private static bool CommandCanExecuteAction(string cmdkey, UICommandParameter<string> parameter)
        {
            return true;
        }

        private async static void CommandExecuteAction(string cmdkey, UICommandParameter<string> parameter)
        {
            switch (cmdkey)
            {
                case "DialogsDemoView":
                    ViewManager.Instance.MainRichView.Show(DialogsDemoView.Instance);
                    break;

                case "FlyoutsDemoView":
                    ViewManager.Instance.MainRichView.SwitchView(FlyoutsDemoView.Instance);
                    break;

                case "ShowMessageDialogBox":
                    {
                        var msresult = MessageDialogBox.Show($"是、否、取消MessageDialogBox，根据需要返回结果，请继续...{Environment.NewLine}[是]：是{Environment.NewLine}[否]：否{Environment.NewLine}[取消]：取消", "提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                        if (msresult == MessageBoxResult.Yes)
                        {
                            MessageDialogBox.Show("你点击了 [是]", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (msresult == MessageBoxResult.No)
                        {
                            MessageDialogBox.Show("你点击了 [否]", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (msresult == MessageBoxResult.Cancel)
                        {
                            MessageDialogBox.Show("你点击了 [取消]，或者直接关闭了", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageDialogBox.Show("不应该返回此结果", "信息", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    break;

                case "ShowProcessDialogBox":
                    {
                        using (ProcessDialogBox box = ProcessDialogBox.Show("测试信息", "标题", false, ProcessBoxButton.Cancel | ProcessBoxButton.PauseContinue | ProcessBoxButton.Stop))
                        using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
                        {
                            bool ispause = false;

                            box.CancelAction = () =>
                            {
                                cancellationTokenSource.Cancel();
                            };
                            box.PauseAction = () =>
                            {
                                ispause = true;
                            };
                            box.StopAction = () =>
                            {
                                cancellationTokenSource.Cancel();
                            };
                            box.ContinueAction = () =>
                            {
                                ispause = false;
                            };

                            await Task.Factory.StartNew(() =>
                            {
                                for (int i = 0; i < 200; i++)
                                {
                                    SpinWait.SpinUntil(() => !ispause || cancellationTokenSource.IsCancellationRequested);
                                    if (cancellationTokenSource.IsCancellationRequested)
                                    {
                                        break;
                                    }

                                    box.ProcessValue = (double)i / 200 * 100;

                                    Thread.Sleep(100);
                                }

                            }, cancellationTokenSource.Token);
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
