using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Linq;
using Unicorn.ViewManager.Preferences;

namespace Unicorn.ViewManager
{
    [TemplatePart(Name = PART_POPUPSTACKPRESENTER, Type = typeof(ContentPresenter))]
    public class PopupStackControl : Control, IPopupItemContainer
    {
        const string PART_POPUPSTACKPRESENTER = "PART_POPUPSTACKPRESENTER";

        private ContentPresenter _stackPresenter = null;
        private readonly PopupStack _popupStack = null;

        public IEnumerable<PopupItem> Items
        {
            get
            {
                return this._popupStack.Items.Cast<PopupItemContainer>().Select(_p => _p.PopupItem);
            }
        }

        public PopupItem TopItem
        {
            get
            {
                return this.PopupItemFromIndex(this._popupStack.Items.Count - 1);
            }
        }

        private readonly PopupItem _parentPopupItem = null;

        static PopupStackControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupStackControl), new FrameworkPropertyMetadata(typeof(PopupStackControl)));

            CommandManager.RegisterClassCommandBinding(typeof(PopupStackControl), new CommandBinding(ViewCommands.ShowPopupItem, new ExecutedRoutedEventHandler(PopupStackControl.OnShowPopupItem), new CanExecuteRoutedEventHandler(PopupStackControl.OnCanShowPopupItem)));
            CommandManager.RegisterClassCommandBinding(typeof(PopupStackControl), new CommandBinding(ViewCommands.ClosePopupItem, new ExecutedRoutedEventHandler(PopupStackControl.OnClosePopupItem), new CanExecuteRoutedEventHandler(PopupStackControl.OnCanClosePopupItem)));
        }

        public PopupStackControl()
        {
            this._popupStack = new PopupStack(this);
        }

        internal PopupStackControl(PopupItem popupItem)
            : this()
        {
            if (popupItem == null)
            {
                throw new ArgumentNullException(nameof(popupItem));
            }

            this._parentPopupItem = popupItem;
        }

        private static void OnCanClosePopupItem(object sender, CanExecuteRoutedEventArgs e)
        {
            PopupStackControl stackControl = (PopupStackControl)sender;
            e.CanExecute = e.Parameter is PopupItem item && stackControl.Contains(item);
            e.Handled = true;
        }

        private static void OnClosePopupItem(object sender, ExecutedRoutedEventArgs e)
        {
            PopupStackControl stackControl = (PopupStackControl)sender;
            stackControl.Close((PopupItem)e.Parameter);
            e.Handled = true;
        }

        private static void OnCanShowPopupItem(object sender, CanExecuteRoutedEventArgs e)
        {
            PopupStackControl stackControl = (PopupStackControl)sender;
            e.CanExecute = e.Parameter is PopupItem item && !stackControl.Contains(item);
            e.Handled = true;
        }

        private static void OnShowPopupItem(object sender, ExecutedRoutedEventArgs e)
        {
            PopupStackControl stackControl = (PopupStackControl)sender;
            stackControl.Show((PopupItem)e.Parameter);
            e.Handled = true;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._stackPresenter = this.GetTemplateChild(PART_POPUPSTACKPRESENTER) as ContentPresenter;

            if (this._stackPresenter == null)
            {
                throw new Exception($"模板缺少名为 {PART_POPUPSTACKPRESENTER} 且类型为 {typeof(ContentPresenter)} 的组件，该组件为视图容器");
            }

            this._stackPresenter.Content = this._popupStack;
        }

        private bool VerifyTopItemModal()
        {
            if (this._popupStack.Items.Count > 0)
            {
                var topitem = this.PopupItemFromIndex(this._popupStack.Items.Count - 1);

                if (topitem._showingAsModal)
                {
                    return true;
                }
            }

            return false;
        }

        private bool VerifyIsMessageDialogBox(PopupItem item)
        {
            return item is MessageDialogBox;
        }

        private bool VerifyIsProcessDialogBox(PopupItem item)
        {
            return item is ProcessDialogBox;
        }

        private bool VerifyIsSpecialItem(PopupItem item)
        {
            return item is MessageDialogBox
                || item is ProcessDialogBox;
        }

        private bool VerifyTopItemIsMessageDialogBox()
        {
            if (this._popupStack.Items.Count > 0)
            {
                var topitem = this.PopupItemFromIndex(this._popupStack.Items.Count - 1);

                return topitem is MessageDialogBox;
            }

            return false;
        }

        public bool Contains(PopupItem item)
        {
            foreach (PopupItemContainer container in this._popupStack.Items)
            {
                if (object.ReferenceEquals(container.PopupItem, item))
                {
                    return true;
                }
            }

            return false;
        }

        internal void VerifyCanShow(PopupItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            //当前是独立作为视图栈
            if (this._parentPopupItem == null)
            {
                if (item._isClosing)
                {
                    throw new Exception("该项当前不可显示，因为它当前正在关闭");
                }

                if (item._isShowing)
                {
                    throw new Exception("该项当前不可显示，因为它当前正在显示");
                }

                if (item._showingAsModal)
                {
                    throw new Exception("该项当前不可显示，因为它当前正在以模态显示");
                }

                if (item._isHostAtViewStack)
                {
                    throw new Exception("该项当前不可显示，因为它当前正在其它视图堆栈中显示");
                }
            }
            else
            {
                //PopupItem 中的视图栈
                if (this.VerifyIsSpecialItem(this._parentPopupItem))
                {
                    throw new Exception($"不可在 {typeof(MessageDialogBox)}、{typeof(ProcessDialogBox)} 等特殊视图中显示子视图");
                }

                if (object.ReferenceEquals(this._parentPopupItem, item))
                {
                    throw new Exception("不可在自己的视图堆栈中显示自己");
                }

                if (this._parentPopupItem._isClosing)
                {
                    throw new Exception("该项当前不可做为容器去显示其它PopupItem，因为它当前正在关闭");
                }

                if (!this._parentPopupItem._isHostAtViewStack)
                {
                    throw new Exception("该项当前不可做为容器去显示其它PopupItem，因为它当前未显示");
                }

                if (this._parentPopupItem._showingAsModal)
                {
                    throw new Exception("该项当前不可做为容器去显示其它PopupItem，因为它当前正在以模态显示");
                }
            }
        }

        private void ShowCore(PopupItem item)
        {
            item.ParentHostStack = this;
            this.AddItem(item);
            item._isShowing = false;

            if (ViewPreferences.Instance.UsePopupViewAnimations)
            {
                PopupItemContainer itemContainer = this.PopupContainerFromItem(item);
                itemContainer.RequestShowAnimation(_p =>
                {
                    try
                    {
                        item.InternalShown(out EventArgs e);
                    }
                    finally
                    {
                        item.InternalDiapose();
                    }
                });
            }
            else
            {
                item.InternalShown(out EventArgs e);
            }
        }

        private void AddItem(PopupItem item)
        {
            PopupItemContainer container = item.GetContainer();
            if (container == null)
            {
                throw new Exception("处理PopupItem容器时出现异常，PopupItem.GetContainer()方法不能返回null");
            }
            container.PopupItem = item;
            this._popupStack.Items.Add(container);
            item._isHostAtViewStack = true;
            item.ParentPopup = this._parentPopupItem;
        }

        private void RemoveItem(PopupItem item)
        {
            var container = this.PopupContainerFromItem(item);
            this._popupStack.Items.Remove(container);
            item._isHostAtViewStack = false;
            item.ParentPopup = null;
        }

        public PopupItemContainer PopupContainerFromIndex(int index)
        {
            return this._popupStack.ItemContainerGenerator.ContainerFromIndex(index) as PopupItemContainer;
        }

        public PopupItemContainer PopupContainerFromItem(PopupItem item)
        {
            foreach (PopupItemContainer container in this._popupStack.Items)
            {
                if (object.ReferenceEquals(container.PopupItem, item))
                {
                    return container;
                }
            }

            return null;
        }

        public PopupItem PopupItemFromIndex(int index)
        {
            return this.PopupContainerFromIndex(index).PopupItem;
        }

        private void MoveItemToTop(PopupItem item)
        {
            if (this.Contains(item)
                && !object.ReferenceEquals(this.TopItem, item))
            {
                PopupItemContainer container = this.PopupContainerFromItem(item);
                this._popupStack.Items.Remove(container);
                this._popupStack.Items.Add(container);
                if (ViewPreferences.Instance.UsePopupViewAnimations)
                {
                    container.RequestShowAnimation(null);
                }
            }
        }

        public ModalResult ShowModal(PopupItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (this.VerifyIsProcessDialogBox(item))
            {
                throw new Exception($"视图类型 {typeof(ProcessDialogBox)} 不可以模态方式进行显示。");
            }

            //以模态显示窗口时，若发现最上层是模态窗口
            //当顶级窗口为MessageDialogBox时不予显示当前窗口
            //当顶级窗口不是MessageDialogBox时，若当前为MessageDialogBox，则显示
            if (this.VerifyTopItemModal())
            {
                if (this.VerifyTopItemIsMessageDialogBox()
                    || !this.VerifyIsMessageDialogBox(item))
                {
                    PopupItemContainer container = this.PopupContainerFromIndex(this._popupStack.Items.Count - 1);
                    if (container != null)
                    {
                        container.Flicker();
                    }

                    return null;
                }
            }

            this.VerifyCanShow(item);

            item._isShowing = true;
            item._showingAsModal = true;
            item._modalResult = null;
            CancelEventArgs ce = null;
            try
            {
                item.InternalShowing(out ce);
            }
            catch (Exception)
            {
                item._isShowing = false;
                item._showingAsModal = false;
                throw;
            }

            bool notcanceled = !ce.Cancel;
            try
            {
                if (notcanceled)
                {
                    ComponentDispatcher.PushModal();
                    item._dispatcherFrame = new DispatcherFrame();
                    this.ShowCore(item);
                    Dispatcher.PushFrame(item._dispatcherFrame);
                    return item.ModalResult;
                }
            }
            finally
            {
                if (notcanceled)
                {
                    ComponentDispatcher.PopModal();
                }

                item.InternalDiapose();
            }

            return null;
        }

        public void Show(PopupItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (this.VerifyIsMessageDialogBox(item))
            {
                throw new Exception($"视图类型 {typeof(MessageDialogBox)} 必须以模态方式进行显示。");
            }

            //堆栈最上层若是模态窗口，则不予显示当前窗口，并激活最上层的模态窗口

            if (this.VerifyTopItemModal())
            {
                if (!this.VerifyIsProcessDialogBox(item))
                {
                    PopupItemContainer container = this.PopupContainerFromIndex(this._popupStack.Items.Count - 1);
                    if (container != null)
                    {
                        container.Flicker();
                    }

                    return;
                }
            }

            if (this.Contains(item))
            {
                this.MoveItemToTop(item);
            }
            else
            {
                this.VerifyCanShow(item);

                item._isShowing = true;
                CancelEventArgs ce = null;
                try
                {
                    item.InternalShowing(out ce);
                }
                catch (Exception)
                {
                    item._isShowing = false;
                    throw;
                }

                if (!ce.Cancel)
                {
                    this.ShowCore(item);
                }
            }
        }

        public void Close(PopupItem item)
        {
            if (item == null)
            {
                return;
            }

            if (item._isClosing
                || !item._isHostAtViewStack)
            {
                return;
            }

            if (!this.Contains(item))
            {
                throw new Exception("该项当前不可关闭，因为它不处于当前的试图堆栈");
            }

            item._isClosing = true;
            CancelEventArgs ce = null;

            try
            {
                item.InternalClosing(out ce);
            }
            catch (Exception)
            {
                item._isClosing = false;
                throw;
            }


            void CloseAndDisposeItem(PopupItem popupItem)
            {
                this.RemoveItem(popupItem);
                popupItem._isClosing = false;
                try
                {
                    popupItem.InternalClosed(out EventArgs e);
                }
                finally
                {
                    popupItem.InternalDiapose();
                }
            }

            //若所属父视图堆栈已关闭，不验证关于取消关闭的操作
            if (item.ParentPopup?._isHostAtViewStack == false)
            {
                CloseAndDisposeItem(item);
            }
            else if (!ce.Cancel)
            {
                if (ViewPreferences.Instance.UsePopupViewAnimations)
                {
                    var container = this.PopupContainerFromItem(item);
                    container.OnCloseAnimation(_p =>
                    {
                        CloseAndDisposeItem(item);
                    });
                }
                else
                {
                    CloseAndDisposeItem(item);
                }
            }
            else
            {
                item._isClosing = false;
            }
        }
    }
}
