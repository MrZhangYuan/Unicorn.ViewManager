<div align="center">
  <h1>Unicorn.ViewManager</h1>
  <p>
    A powerful WPF view management library
  </p>
  <p>
    Supporting .NET Core (3.0 and 3.1)
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
The second step is to initialize a Unicorn.ViewManager.Dialog or Unicorn.ViewManager.Flyout,you can inherit it in xaml,when you have a Unicorn.ViewManager.Dialog or Unicorn.ViewManager.Flyout, you can display it with ViewManager.Instance.Show(PopupItem item) method.
```csharp
ViewManager.Instance.Show(new FullScreenDialog());--FullScreenDialog is my Dialog instance inherit Dialog
--OR
ViewManager.Instance.Show(new LeftFlyout());--LeftFlyout is my flyout instance inherit Flyout
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
