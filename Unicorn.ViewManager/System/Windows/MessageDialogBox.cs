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
using System.Windows.Threading;
using Unicorn.ViewManager;

namespace System.Windows
{
    [TemplatePart(Name = PART_CLOSEBUTTON, Type = typeof(Button))]
    [TemplatePart(Name = PART_OKBUTTON, Type = typeof(Button))]
    [TemplatePart(Name = PART_YESBUTTON, Type = typeof(Button))]
    [TemplatePart(Name = PART_NOBUTTON, Type = typeof(Button))]
    [TemplatePart(Name = PART_CANCELBUTTON, Type = typeof(Button))]
    public sealed class MessageDialogBox : Dialog
    {
        private const string PART_CLOSEBUTTON = "PART_CLOSEBUTTON";
        private const string PART_OKBUTTON = "PART_OKBUTTON";
        private const string PART_YESBUTTON = "PART_YESBUTTON";
        private const string PART_NOBUTTON = "PART_NOBUTTON";
        private const string PART_CANCELBUTTON = "PART_CANCELBUTTON";


        private IPopupItemContainer _owner = null;

        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }
            private set
            {
                SetValue(CaptionProperty, value);
            }
        }
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(MessageDialogBox), new PropertyMetadata(string.Empty));

        public string MessageText
        {
            get
            {
                return (string)GetValue(MessageTextProperty);
            }
            private set
            {
                SetValue(MessageTextProperty, value);
            }
        }
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register("MessageText", typeof(string), typeof(MessageDialogBox), new PropertyMetadata(string.Empty));

        public MessageBoxButton MessageBoxButton
        {
            get
            {
                return (MessageBoxButton)GetValue(MessageBoxButtonProperty);
            }
            private set
            {
                SetValue(MessageBoxButtonProperty, value);
            }
        }
        public static readonly DependencyProperty MessageBoxButtonProperty = DependencyProperty.Register("MessageBoxButton", typeof(MessageBoxButton), typeof(MessageDialogBox), new PropertyMetadata(MessageBoxButton.OK, MessageBoxButtonPropertyChangedCallBack));

        public MessageBoxImage MessageBoxImage
        {
            get
            {
                return (MessageBoxImage)GetValue(MessageBoxImageProperty);
            }
            private set
            {
                SetValue(MessageBoxImageProperty, value);
            }
        }
        public static readonly DependencyProperty MessageBoxImageProperty = DependencyProperty.Register("MessageBoxImage", typeof(MessageBoxImage), typeof(MessageDialogBox), new PropertyMetadata(MessageBoxImage.None));

        public MessageBoxResult DefaultResult
        {
            get
            {
                return (MessageBoxResult)GetValue(DefaultResultProperty);
            }
            private set
            {
                SetValue(DefaultResultProperty, value);
            }
        }
        public static readonly DependencyProperty DefaultResultProperty = DependencyProperty.Register("DefaultResult", typeof(MessageBoxResult), typeof(MessageDialogBox), new PropertyMetadata(MessageBoxResult.None));

        public MessageBoxOptions MessageBoxOptions
        {
            get
            {
                return (MessageBoxOptions)GetValue(MessageBoxOptionsProperty);
            }
            private set
            {
                SetValue(MessageBoxOptionsProperty, value);
            }
        }
        public static readonly DependencyProperty MessageBoxOptionsProperty = DependencyProperty.Register("MessageBoxOptions", typeof(MessageBoxOptions), typeof(MessageDialogBox), new PropertyMetadata(MessageBoxOptions.None));

        static MessageDialogBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialogBox), new FrameworkPropertyMetadata(typeof(MessageDialogBox)));
        }

        private MessageDialogBox()
        {

        }

        private Button _closeBt = null,
            _okBt = null,
            _yesBt = null,
            _noBt = null,
            _cancelBt = null;
        private bool _isTemplateApply = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _closeBt = this.GetTemplateChild(PART_CLOSEBUTTON) as Button;
            _okBt = this.GetTemplateChild(PART_OKBUTTON) as Button;
            _yesBt = this.GetTemplateChild(PART_YESBUTTON) as Button;
            _noBt = this.GetTemplateChild(PART_NOBUTTON) as Button;
            _cancelBt = this.GetTemplateChild(PART_CANCELBUTTON) as Button;

            if (this._closeBt == null
                || this._okBt == null
                || this._yesBt == null
                || this._noBt == null
                || this._cancelBt == null)
            {
                throw new Exception($"模板缺少必须元素{PART_CLOSEBUTTON}、{PART_OKBUTTON}、{PART_YESBUTTON}、{PART_NOBUTTON}、{PART_CANCELBUTTON}一个或多个");
            }

            this._isTemplateApply = true;

            this.RefreshMessageBoxButtonVisibility();

            this._closeBt.Click += (sender, e) =>
            {
                if (this.ValidateButtonUsed(_BoxButton.Close))
                {
                    this.ModalResult = new ModalResult { Result = this.ConverterDefaultResult() };
                }
            };
            this._okBt.Click += (sender, e) =>
            {
                if (this.ValidateButtonUsed(_BoxButton.OK))
                {
                    this.ModalResult = new ModalResult { Result = MessageBoxResult.OK };
                }
            };
            this._yesBt.Click += (sender, e) =>
            {
                if (this.ValidateButtonUsed(_BoxButton.Yes))
                {
                    this.ModalResult = new ModalResult { Result = MessageBoxResult.Yes };
                }
            };
            this._noBt.Click += (sender, e) =>
            {
                if (this.ValidateButtonUsed(_BoxButton.No))
                {
                    this.ModalResult = new ModalResult { Result = MessageBoxResult.No };
                }
            };
            this._cancelBt.Click += (sender, e) =>
            {
                if (this.ValidateButtonUsed(_BoxButton.Cancel))
                {
                    this.ModalResult = new ModalResult { Result = MessageBoxResult.Cancel };
                }
            };
        }

        private static void MessageBoxButtonPropertyChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialogBox)d).RefreshMessageBoxButtonVisibility();
        }

        private enum _BoxButton
        {
            Close,
            OK,
            Yes,
            No,
            Cancel
        }
        private void RefreshMessageBoxButtonVisibility()
        {
            if (this._isTemplateApply)
            {
                switch (this.MessageBoxButton)
                {
                    case MessageBoxButton.OK:
                        this._okBt.Visibility = Visibility.Visible;
                        this._okBt.IsEnabled = true;
                        this._closeBt.Visibility = Visibility.Visible;
                        this._closeBt.IsEnabled = true;

                        this._yesBt.Visibility = Visibility.Collapsed;
                        this._yesBt.IsEnabled = false;
                        this._noBt.Visibility = Visibility.Collapsed;
                        this._noBt.IsEnabled = false;
                        this._cancelBt.Visibility = Visibility.Collapsed;
                        this._cancelBt.IsEnabled = false;
                        break;

                    case MessageBoxButton.OKCancel:
                        this._okBt.Visibility = Visibility.Visible;
                        this._okBt.IsEnabled = true;
                        this._closeBt.Visibility = Visibility.Visible;
                        this._closeBt.IsEnabled = true;
                        this._cancelBt.Visibility = Visibility.Visible;
                        this._cancelBt.IsEnabled = true;

                        this._yesBt.Visibility = Visibility.Collapsed;
                        this._yesBt.IsEnabled = false;
                        this._noBt.Visibility = Visibility.Collapsed;
                        this._noBt.IsEnabled = false;
                        break;

                    case MessageBoxButton.YesNoCancel:
                        this._yesBt.Visibility = Visibility.Visible;
                        this._yesBt.IsEnabled = true;
                        this._noBt.Visibility = Visibility.Visible;
                        this._noBt.IsEnabled = true;
                        this._closeBt.Visibility = Visibility.Visible;
                        this._closeBt.IsEnabled = true;
                        this._cancelBt.Visibility = Visibility.Visible;
                        this._cancelBt.IsEnabled = true;

                        this._okBt.Visibility = Visibility.Collapsed;
                        this._okBt.IsEnabled = false;
                        break;

                    case MessageBoxButton.YesNo:
                        this._yesBt.Visibility = Visibility.Visible;
                        this._yesBt.IsEnabled = true;
                        this._noBt.Visibility = Visibility.Visible;
                        this._noBt.IsEnabled = true;

                        this._closeBt.Visibility = Visibility.Collapsed;
                        this._closeBt.IsEnabled = false;
                        this._cancelBt.Visibility = Visibility.Collapsed;
                        this._cancelBt.IsEnabled = false;
                        this._okBt.Visibility = Visibility.Collapsed;
                        this._okBt.IsEnabled = false;
                        break;
                }
            }
        }

        private bool ValidateButtonUsed(_BoxButton button)
        {
            switch (button)
            {
                case _BoxButton.Close:
                    return this.MessageBoxButton != MessageBoxButton.YesNo;

                case _BoxButton.OK:
                    return this.MessageBoxButton == MessageBoxButton.OK
                         || this.MessageBoxButton == MessageBoxButton.OKCancel;

                case _BoxButton.Yes:
                    return this.MessageBoxButton == MessageBoxButton.YesNo
                        || this.MessageBoxButton == MessageBoxButton.YesNoCancel;

                case _BoxButton.No:
                    return this.MessageBoxButton == MessageBoxButton.YesNo
                        || this.MessageBoxButton == MessageBoxButton.YesNoCancel;

                case _BoxButton.Cancel:
                    return this.MessageBoxButton == MessageBoxButton.OKCancel
                        || this.MessageBoxButton == MessageBoxButton.YesNoCancel;
            }

            return false;
        }

        private MessageBoxResult ConverterDefaultResult()
        {
            MessageBoxResult result = MessageBoxResult.Cancel;

            switch (this.MessageBoxButton)
            {
                case MessageBoxButton.OK:
                    result = MessageBoxResult.OK;
                    break;

                case MessageBoxButton.OKCancel:
                    result = MessageBoxResult.Cancel;
                    break;

                case MessageBoxButton.YesNoCancel:
                    result = MessageBoxResult.Cancel;
                    break;

                case MessageBoxButton.YesNo:
                    result = MessageBoxResult.No;
                    break;
            }

            return result;
        }

        public static MessageBoxResult Show(string messageBoxText)
        {
            return ShowCore(null, messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return ShowCore(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return ShowCore(null, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowCore(null, messageBoxText, caption, button, icon, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return ShowCore(null, messageBoxText, caption, button, icon, defaultResult, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            return ShowCore(null, messageBoxText, caption, button, icon, defaultResult, options);
        }
        public static MessageBoxResult Show(IPopupItemContainer owner, string messageBoxText)
        {
            return ShowCore(owner, messageBoxText, null, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(IPopupItemContainer owner, string messageBoxText, string caption)
        {
            return ShowCore(owner, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(IPopupItemContainer owner, string messageBoxText, string caption, MessageBoxButton button)
        {
            return ShowCore(owner, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(IPopupItemContainer owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return ShowCore(owner, messageBoxText, caption, button, icon, MessageBoxResult.None, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(IPopupItemContainer owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult, MessageBoxOptions.None);
        }
        public static MessageBoxResult Show(IPopupItemContainer owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult, options);
        }

        private static MessageBoxResult ShowCore(IPopupItemContainer owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                if (owner == null)
                {
                    owner = ViewManager.Instance.MainRichView;
                }

                MessageDialogBox messageDialogBox = new MessageDialogBox
                {
                    MessageText = messageBoxText,
                    Caption = caption,
                    MessageBoxButton = button,
                    MessageBoxImage = icon,
                    DefaultResult = defaultResult,
                    MessageBoxOptions = options
                };
                messageDialogBox._owner = owner;

                var modalresult = owner.ShowModal(messageDialogBox);
                if (modalresult == null)
                {
                    return messageDialogBox.ConverterDefaultResult();
                }
                return (MessageBoxResult)modalresult.Result;

            }, DispatcherPriority.Send);
        }
    }
}
