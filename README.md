<div align="center">
  <h1>Unicorn.ViewManager</h1>
  <p>
    A powerful WPF view management library
  </p>
  <p>
    Supporting .NET Core (3.1)
  </p>
</div>


## Installation
- [Nuget](https://www.nuget.org/packages/Unicorn.ViewManager/)

```
PM> Install-Package Unicorn.ViewManager
```

## Let's get started
To start, first add this code at MainWindow's constructor.This step is to initialize a main view stack. ViewManager.Instance.MainRichView.SwitchView(object) method is used to switch the main view in the program,it does not belong to the view stack.
```csharp
public MainWindow()
{
  InitializeComponent();
  
  ViewManager.Instance.InitializeRichView(this);
  ViewManager.Instance.ViewPreferences.UsePopupViewAnimations = true;
  ViewManager.Instance.MainRichView.SwitchView(new MainView());
}
```
The second step is to initialize a Unicorn.ViewManager.Dialog or Unicorn.ViewManager.Flyout,you can inherit it in xaml,when you have a Unicorn.ViewManager.Dialog or Unicorn.ViewManager.Flyout,.
you can display it with ViewManager.Instance.Show(PopupItem item) method.
```csharp
ViewManager.Instance.Show(new FullScreenDialog());//FullScreenDialog is my Dialog instance inherit Dialog
//OR
new FullScreenDialog().Show();//at default main view stack
//OR
new FullScreenDialog().Show(IPopupItemContainer );//IPopupItemContainer is a interface
//OR
new FullScreenDialog().ShowAsModal();//at default main view stack
//OR
new FullScreenDialog().ShowAsModal(IPopupItemContainer );//at custom IPopupItemContainer
```
you can close it like that.
```csharp
ViewManager.Instance.Close(Dialog dialog);//FullScreenDialog is my Dialog instance inherit Dialog
//OR
dialog.Close();
```

## View's events
Each view item has four events, they are 'Showing','Shown','Closing','Closed'.
'Showing','Closing' can be canceled. you can use it like that:
```csharp
public partial class DialogWithEvent : Dialog
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
}
```
See more details in the demo.


## Screenshots
![Overview](https://github.com/MrZhangYuan/ViewManagerResources/blob/master/Resources/Images/ReadMe.png)
![Overview](https://github.com/MrZhangYuan/ViewManagerResources/blob/master/Resources/Images/Dialogs.png)
![Overview](https://github.com/MrZhangYuan/ViewManagerResources/blob/master/Resources/Images/FullScreenDialog.png)
![Overview](https://github.com/MrZhangYuan/ViewManagerResources/blob/master/Resources/Images/ModalDialog.png)
![Overview](https://github.com/MrZhangYuan/ViewManagerResources/blob/master/Resources/Images/NormalDialog.png)
![Overview](https://github.com/MrZhangYuan/ViewManagerResources/blob/master/Resources/Images/MessageDiagBox.png)
![Overview](https://github.com/MrZhangYuan/ViewManagerResources/blob/master/Resources/Images/ProcessDiagBox.png)
