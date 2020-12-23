using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    [Flags]
    public enum ProcessBoxButton
    {
        None = 0x0,
        Cancel = 0x1,
        PauseContinue = 0x2,
        Stop = 0x4
    }

    public enum ProcessBoxImage
    {
        None
    }


    [TemplatePart(Name = PART_CANCELBUTTON, Type = typeof(Button))]
    [TemplatePart(Name = PART_PAUSEBUTTON, Type = typeof(Button))]
    [TemplatePart(Name = PART_CONTINUEBUTTON, Type = typeof(Button))]
    [TemplatePart(Name = PART_STOPBUTTON, Type = typeof(Button))]
    public sealed class ProcessDialogBox : Dialog, IDisposable
    {
        private const string PART_CANCELBUTTON = "PART_CANCELBUTTON";
        private const string PART_PAUSEBUTTON = "PART_PAUSEBUTTON";
        private const string PART_CONTINUEBUTTON = "PART_CONTINUEBUTTON";
        private const string PART_STOPBUTTON = "PART_STOPBUTTON";

        static ProcessDialogBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ProcessDialogBox), new FrameworkPropertyMetadata(typeof(ProcessDialogBox)));
        }

        public bool IsIndeterminate
        {
            get
            {
                return (bool)GetValue(IsIndeterminateProperty);
            }
            private set
            {
                SetValue(IsIndeterminateProperty, value);
            }
        }
        public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register("IsIndeterminate", typeof(bool), typeof(ProcessDialogBox), new PropertyMetadata(true));

        public string Caption
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    return (string)GetValue(CaptionProperty);
                });
            }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    SetValue(CaptionProperty, value);
                });
            }
        }
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(ProcessDialogBox), new PropertyMetadata(string.Empty));

        public string MessageText
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    return (string)GetValue(MessageTextProperty);
                });
            }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    SetValue(MessageTextProperty, value);
                });
            }
        }
        public static readonly DependencyProperty MessageTextProperty = DependencyProperty.Register("MessageText", typeof(string), typeof(ProcessDialogBox), new PropertyMetadata(string.Empty));


        public ProcessBoxButton ProcessBoxButton
        {
            get
            {
                return (ProcessBoxButton)GetValue(ProcessBoxButtonProperty);
            }
            private set
            {
                this.ThrowIfFreeze();
                SetValue(ProcessBoxButtonProperty, value);
            }
        }
        public static readonly DependencyProperty ProcessBoxButtonProperty = DependencyProperty.Register("ProcessBoxButton", typeof(ProcessBoxButton), typeof(ProcessDialogBox), new PropertyMetadata(ProcessBoxButton.None, ProcessBoxButtonChangedCallBack));

        private static void ProcessBoxButtonChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ProcessDialogBox)d).RefrehButtonVisibility();
        }

        public ProcessBoxImage ProcessBoxImage
        {
            get
            {
                return (ProcessBoxImage)GetValue(ProcessBoxImageProperty);
            }
            private set
            {
                SetValue(ProcessBoxImageProperty, value);
            }
        }
        public static readonly DependencyProperty ProcessBoxImageProperty = DependencyProperty.Register("ProcessBoxImage", typeof(ProcessBoxImage), typeof(ProcessDialogBox), new PropertyMetadata(ProcessBoxImage.None));

        public double ProcessValue
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    return (double)GetValue(ProcessValueProperty);
                });
            }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    SetValue(ProcessValueProperty, value);
                });
            }
        }
        public static readonly DependencyProperty ProcessValueProperty = DependencyProperty.Register("ProcessValue", typeof(double), typeof(ProcessDialogBox), new PropertyMetadata(0d));


        public double MaxProcess
        {
            get
            {
                return this.Dispatcher.Invoke(() =>
                {
                    return (double)GetValue(MaxProcessProperty);
                });
            }
            set
            {
                this.Dispatcher.Invoke(() =>
                {
                    SetValue(MaxProcessProperty, value);
                });
            }
        }
        public static readonly DependencyProperty MaxProcessProperty = DependencyProperty.Register("MaxProcess", typeof(double), typeof(ProcessDialogBox), new PropertyMetadata(100d));


        public void ReportProcess(double maxprocess, double processvalue)
        {
            this.ProcessValue = processvalue;
            this.MaxProcess = maxprocess;
        }

        private ProcessDialogBox()
        {

        }

        public void ThrowIfFreeze()
        {
            if (this._isFreezed)
            {
                throw new Exception("当前不可修改");
            }
        }

        private enum _ProcessStatus
        {
            NotStarted,
            Canceled,
            Running,
            Paused,
            Stoped
        }
        private _ProcessStatus _processStatus = _ProcessStatus.NotStarted;
        private Button _pauseBt = null,
          _continueBt = null,
          _stopBt = null,
          _cancelBt = null;
        private bool _isTemplateApply = false;
        private bool _isFreezed = false;
        private IPopupItemContainer _owner = null;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _pauseBt = this.GetTemplateChild(PART_PAUSEBUTTON) as Button;
            _continueBt = this.GetTemplateChild(PART_CONTINUEBUTTON) as Button;
            _stopBt = this.GetTemplateChild(PART_STOPBUTTON) as Button;
            _cancelBt = this.GetTemplateChild(PART_CANCELBUTTON) as Button;

            if (this._pauseBt == null
                || this._continueBt == null
                || this._stopBt == null
                || this._cancelBt == null)
            {
                throw new Exception($"模板缺少必须元素{PART_PAUSEBUTTON}、{PART_CONTINUEBUTTON}、{PART_STOPBUTTON}、{PART_CANCELBUTTON}一个或多个");
            }

            this._isTemplateApply = true;

            this.RefrehButtonVisibility();

            this._pauseBt.Click += (sender, e) =>
            {
                if (this.PauseAction == null)
                {
                    return;
                }

                try
                {
                    this.PauseAction();
                }
                finally
                {
                    this._pauseBt.Visibility = Visibility.Collapsed;
                    this._pauseBt.IsEnabled = false;

                    this._continueBt.Visibility = Visibility.Visible;
                    this._continueBt.IsEnabled = true;

                    this._processStatus = _ProcessStatus.Paused;
                }
            };

            this._continueBt.Click += (sender, e) =>
            {
                if (this.ContinueAction == null)
                {
                    return;
                }

                try
                {
                    this.ContinueAction();
                }
                finally
                {
                    this._pauseBt.Visibility = Visibility.Visible;
                    this._pauseBt.IsEnabled = true;

                    this._continueBt.Visibility = Visibility.Collapsed;
                    this._continueBt.IsEnabled = false;

                    this._processStatus = _ProcessStatus.Running;
                }
            };

            this._stopBt.Click += (sender, e) =>
            {
                if (this.StopAction == null)
                {
                    return;
                }

                try
                {
                    this.StopAction();
                }
                finally
                {
                    this._processStatus = _ProcessStatus.Stoped;
                    this.Dispose();
                }
            };

            this._cancelBt.Click += (sender, e) =>
            {
                if (this.CancelAction == null)
                {
                    return;
                }

                try
                {
                    this.CancelAction();
                }
                finally
                {
                    this._processStatus = _ProcessStatus.Canceled;
                    this.Dispose();
                }
            };
        }

        private static bool IsFlagSet(ProcessBoxButton flag, ProcessBoxButton flags)
        {
            return (uint)(flags & flag) > 0U;
        }

        private void RefrehButtonVisibility()
        {
            if (!this._isTemplateApply)
            {
                return;
            }

            this._pauseBt.Visibility = Visibility.Collapsed;
            this._pauseBt.IsEnabled = false;

            this._continueBt.Visibility = Visibility.Collapsed;
            this._continueBt.IsEnabled = false;

            this._stopBt.Visibility = Visibility.Collapsed;
            this._stopBt.IsEnabled = false;

            this._cancelBt.Visibility = Visibility.Collapsed;
            this._cancelBt.IsEnabled = false;

            if (this.ProcessBoxButton != ProcessBoxButton.None)
            {
                if (IsFlagSet(ProcessBoxButton.Cancel, this.ProcessBoxButton))
                {
                    this._cancelBt.Visibility = Visibility.Visible;
                    this._cancelBt.IsEnabled = true;
                }

                if (IsFlagSet(ProcessBoxButton.PauseContinue, this.ProcessBoxButton))
                {
                    this._pauseBt.Visibility = Visibility.Visible;
                    this._pauseBt.IsEnabled = true;
                }

                if (IsFlagSet(ProcessBoxButton.Stop, this.ProcessBoxButton))
                {
                    this._stopBt.Visibility = Visibility.Visible;
                    this._stopBt.IsEnabled = true;
                }
            }

            this._isFreezed = true;
        }

        public Action CancelAction
        {
            get; set;
        }

        public Action PauseAction
        {
            get; set;
        }

        public Action ContinueAction
        {
            get; set;
        }

        public Action StopAction
        {
            get; set;
        }

        public void Dispose()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                this._isFreezed = false;
                this._owner.Close(this);
            }, DispatcherPriority.Send);
        }

        public static ProcessDialogBox Show(string text, string caption)
        {
            return ShowCore(null, text, caption, true, ProcessBoxButton.None, ProcessBoxImage.None);
        }
        public static ProcessDialogBox Show(string text, string caption, bool isindeterminate)
        {
            return ShowCore(null, text, caption, isindeterminate, ProcessBoxButton.None, ProcessBoxImage.None);
        }
        public static ProcessDialogBox Show(string text, string caption, bool isindeterminate, ProcessBoxButton processBoxButton)
        {
            return ShowCore(null, text, caption, isindeterminate, processBoxButton, ProcessBoxImage.None);
        }
        public static ProcessDialogBox Show(string text, string caption, bool isindeterminate, ProcessBoxButton processBoxButton, ProcessBoxImage processBoxImage)
        {
            return ShowCore(null, text, caption, isindeterminate, processBoxButton, processBoxImage);
        }

        public static ProcessDialogBox Show(IPopupItemContainer owner, string text, string caption)
        {
            return ShowCore(owner, text, caption, true, ProcessBoxButton.None, ProcessBoxImage.None);
        }
        public static ProcessDialogBox Show(IPopupItemContainer owner, string text, string caption, bool isindeterminate)
        {
            return ShowCore(owner, text, caption, isindeterminate, ProcessBoxButton.None, ProcessBoxImage.None);
        }
        public static ProcessDialogBox Show(IPopupItemContainer owner, string text, string caption, bool isindeterminate, ProcessBoxButton processBoxButton)
        {
            return ShowCore(owner, text, caption, isindeterminate, processBoxButton, ProcessBoxImage.None);
        }
        public static ProcessDialogBox Show(IPopupItemContainer owner, string text, string caption, bool isindeterminate, ProcessBoxButton processBoxButton, ProcessBoxImage processBoxImage)
        {
            return ShowCore(owner, text, caption, isindeterminate, processBoxButton, processBoxImage);
        }

        private static ProcessDialogBox ShowCore(IPopupItemContainer owner, string text, string caption, bool isindeterminate, ProcessBoxButton processBoxButton, ProcessBoxImage processBoxImage)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                if (owner == null)
                {
                    owner = ViewManager.Instance.ActiveContainer;
                }

                ProcessDialogBox processDialogBox = new ProcessDialogBox
                {
                    MessageText = text,
                    Caption = caption,
                    IsIndeterminate = isindeterminate,
                    ProcessBoxButton = processBoxButton,
                    ProcessBoxImage = processBoxImage
                };

                processDialogBox._owner = owner;

                processDialogBox._owner.Show(processDialogBox);

                return processDialogBox;

            }, DispatcherPriority.Send);
        }
    }
}
