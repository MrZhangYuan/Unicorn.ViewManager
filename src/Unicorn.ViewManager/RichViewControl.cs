using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.Utilities;

namespace Unicorn.ViewManager
{
    public interface IRichViewContainer
    {
        void ShowView(object item);
        void CloseView(object item);
        void SwitchView(object item);
    }

    [TemplatePart(Name = PART_POPUPSTACKCONTROL, Type = typeof(ContentPresenter))]
    public class RichViewControl : ItemsControl, IRichViewContainer, IPopupItemContainer
    {
        private const string PART_POPUPSTACKCONTROL = "PART_POPUPSTACKCONTROL";

        private readonly PopupStackControl _popupStackControl = null;

        public PopupStackControl PopupStackControl
        {
            get
            {
                return this._popupStackControl;
            }
        }

        public PopupItem TopItem => this.PopupStackControl.TopItem;

        IPopupItemContainer IPopupItemContainer.Parent => ((IPopupItemContainer)this.PopupStackControl).Parent;
      
        public IEnumerable<PopupItem> Children => this.PopupStackControl.Children;

        public event ViewStackChangedEventHandler ViewStackChanged
        {
            add
            {
                this.PopupStackControl.ViewStackChanged += value;
            }
            remove
            {
                this.PopupStackControl.ViewStackChanged -= value;
            }
        }

        public bool IsAnimationActive
        {
            get
            {
                return (bool)GetValue(IsAnimationActiveProperty);
            }
            set
            {
                SetValue(IsAnimationActiveProperty, value);
            }
        }
        public static readonly DependencyProperty IsAnimationActiveProperty = ProgressRing.IsAnimationActiveProperty.AddOwner(typeof(RichViewControl), new PropertyMetadata(false));


        static RichViewControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RichViewControl), new FrameworkPropertyMetadata(typeof(RichViewControl)));
            CommandManager.RegisterClassCommandBinding(typeof(RichViewControl), new CommandBinding(ViewCommands.ShowView, new ExecutedRoutedEventHandler(RichViewControl.OnShowView), new CanExecuteRoutedEventHandler(RichViewControl.OnCanShowView)));
            CommandManager.RegisterClassCommandBinding(typeof(RichViewControl), new CommandBinding(ViewCommands.CloseView, new ExecutedRoutedEventHandler(RichViewControl.OnCloseView), new CanExecuteRoutedEventHandler(RichViewControl.OnCanCloseView)));
            CommandManager.RegisterClassCommandBinding(typeof(RichViewControl), new CommandBinding(ViewCommands.SwitchView, new ExecutedRoutedEventHandler(RichViewControl.OnSwitchView), new CanExecuteRoutedEventHandler(RichViewControl.OnCanSwitchView)));
        }

        public RichViewControl()
        {
            this._popupStackControl = new PopupStackControl();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var contentPresenter = GetTemplateChild(PART_POPUPSTACKCONTROL) as ContentPresenter;

            if (contentPresenter != null)
            {
                contentPresenter.Content = this._popupStackControl;
            }
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is RichViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new RichViewItem();
        }

        public ModalResult ShowModal(PopupItem item)
        {
            return this._popupStackControl.ShowModal(item);
        }

        public void Show(PopupItem item)
        {
            this._popupStackControl.Show(item);
        }

        public void Close(PopupItem item)
        {
            this._popupStackControl.Close(item);
        }

        public void ShowView(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!this.Items.Contains(item))
            {
                this.Items.Add(item);
            }
        }

        public void CloseView(object item)
        {
            if (item != null)
            {
                this.Items.Remove(item);
            }
        }

        public void SwitchView(object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.ShowView(item);

            if (this.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                foreach (var view in this.Items)
                {
                    var container = this.ItemContainerGenerator.ContainerFromItem(view) as Control;

                    if (object.ReferenceEquals(view, item))
                    {
                        container.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        container.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private static void OnCanShowView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
            e.Handled = true;
        }

        private static void OnShowView(object sender, ExecutedRoutedEventArgs e)
        {
            ((RichViewControl)sender).ShowView(e.Parameter);
        }

        private static void OnCanCloseView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
            e.Handled = true;
        }

        private static void OnCloseView(object sender, ExecutedRoutedEventArgs e)
        {
            ((RichViewControl)sender).CloseView(e.Parameter);
        }

        private static void OnCanSwitchView(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
            e.Handled = true;
        }

        private static void OnSwitchView(object sender, ExecutedRoutedEventArgs e)
        {
            ((RichViewControl)sender).SwitchView(e.Parameter);
        }

        public bool Close()
        {
            return this._popupStackControl.Close();
        }
    }
}
