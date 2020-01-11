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
    /// DialogWithEvent.xaml 的交互逻辑
    /// </summary>
    public partial class DialogWithEvent
    {
        public DialogWithEvent()
        {
            InitializeComponent();
            this.Showing += DialogWithEvent_Showing;
            this.Shown += DialogWithEvent_Shown;
            this.Closing += DialogWithEvent_Closing;
            this.Closed += DialogWithEvent_Closed;
        }

        private void DialogWithEvent_Closed(object sender, EventArgs e)
        {
            MessageDialogBox.Show("该Dialog已彻底关闭，并已从视图栈移除，可在此事件处理中处理一些释放问题", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DialogWithEvent_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var msresult = MessageDialogBox.Show("Dialog With Event 准备关闭，此时可以取消，是否继续关闭", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msresult == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void DialogWithEvent_Shown(object sender, EventArgs e)
        {
            MessageDialogBox.Show("该Dialog已显示完成，可在此事件处理中处理一些加载问题", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DialogWithEvent_Showing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var msresult = MessageDialogBox.Show("Dialog With Event 准备显示，此时可以取消，是否继续显示", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msresult == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void StackPanel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
