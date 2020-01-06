using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unicorn.Utilities;

namespace Unicorn.ViewManager
{
    internal class DialogContainer : PopupItemContainer
    {
        public Dialog Dialog
        {
            get
            {
                return (Dialog)GetValue(DialogProperty);
            }
            set
            {
                SetValue(DialogProperty, value);
            }
        }
        public static readonly DependencyProperty DialogProperty = DependencyProperty.Register("Dialog", typeof(Dialog), typeof(DialogContainer), new PropertyMetadata(null));

        static DialogContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DialogContainer), new FrameworkPropertyMetadata(typeof(DialogContainer)));
        }

        private Border _border = null;
        private Border _funcBorder = null;
        private Border _transformBorder = null;
        private ContentPresenter _content;
        private bool _isTemplateApplyed = false;
        public override void OnApplyTemplate()
        {
            this._border = this.GetTemplateChild("PART_BACKGROUND") as Border;
            this._funcBorder = this.GetTemplateChild("_funcBorder") as Border;
            this._transformBorder = this.GetTemplateChild("_transformBorder") as Border;
            this._content = this.GetTemplateChild("PART_CONTENT") as ContentPresenter;
            this._isTemplateApplyed = true;

            this._funcBorder.MouseDown += _funcBorder_MouseDown;

            base.OnApplyTemplate();
        }

        private void _funcBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender
                && e.OriginalSource is Border
                && !this.PopupItem._isClosing)
            {
                if (this.PopupItem._showingAsModal)
                {
                    this.Flicker();
                }
                else if (this.PopupItem.IsEasyClose)
                {
                    this.PopupItem.Close();
                }
            }

            e.Handled = true;
        }

        private void RestoreTransform()
        {
            if (this._transformBorder != null)
            {
                this._transformBorder.RenderTransform = null;
            }
        }

        private bool _isCloseInvoked = false;

        protected internal override void OnShowAnimation(Action<PopupItem> callback)
        {
            this.RestoreTransform();

            if (ViewManager.Instance.ViewPreferences.UsePopupViewAnimations
                && this._isTemplateApplyed)
            {
                this._border.Opacity = 0;
                this._border.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.ControlOpacityToOne,
                    Values = new TransformValues(),
                    StoryboardComplateCallBack = obj =>
                    {
                        if (!this._isCloseInvoked)
                        {
                            this._border.BeginAnimation(Control.OpacityProperty, null);
                            this._border.Opacity = 1;
                        }

                        base.OnShowAnimation(callback);
                    }
                });

                var actualwidth = this.ActualWidth == 0 ? this.Dialog.ParentHostStack.ActualWidth : this.ActualWidth;
                var actualheigth = this.ActualHeight == 0 ? this.Dialog.ParentHostStack.ActualHeight : this.ActualHeight;

                this._transformBorder.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.ScaleTransformToOne,
                    Values = new TransformValues
                    {
                        ScaleCenterX = actualwidth / 2,
                        ScaleCenterY = actualheigth / 2,
                        ScaleFromX = 0.9,
                        ScaleFromY = 0.9,

                        //ScaleFromX = 1.2,
                        //ScaleFromY = 1.2,
                    }
                });
            }
            else
            {
                base.OnShowAnimation(callback);
            }
        }

        protected internal override void OnCloseAnimation(Action<PopupItem> callback)
        {
            if (this._isCloseInvoked)
            {
                return;
            }
            this._isCloseInvoked = true;

            this.RestoreTransform();

            if (ViewManager.Instance.ViewPreferences.UsePopupViewAnimations
                && this._isTemplateApplyed)
            {
                this._border.IsHitTestVisible = false;

                this._border.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.ControlOpacityToZero,
                    Values = new TransformValues(),
                    StoryboardComplateCallBack = obj =>
                    {
                        this._border.BeginAnimation(Control.OpacityProperty, null);
                        this._border.Opacity = 0;

                        base.OnCloseAnimation(callback);
                    }
                });

                var actualwidth = this.ActualWidth == 0 ? this.Dialog.ParentHostStack.ActualWidth : this.ActualWidth;
                var actualheigth = this.ActualHeight == 0 ? this.Dialog.ParentHostStack.ActualHeight : this.ActualHeight;

                this._transformBorder.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.ScaleTransformToValue,
                    Values = new TransformValues
                    {
                        ScaleCenterX = actualwidth / 2,
                        ScaleCenterY = actualheigth / 2,
                        ScaleToX = 0.9,
                        ScaleToY = 0.9
                    }
                });
            }
            else
            {
                base.OnCloseAnimation(callback);
            }
        }

        protected async internal override void Flicker()
        {
            base.Flicker();

            if (this._isTemplateApplyed)
            {
                DropShadowEffect effect = new DropShadowEffect
                {
                    Color = Colors.Black,
                    Opacity = 0.8,
                    ShadowDepth = 0,
                    BlurRadius = 20
                };

                int delay = 40;

                this._content.Effect = effect;
                await Task.Delay(delay);
                this._content.Effect = null;
                await Task.Delay(delay);
                this._content.Effect = effect;
                await Task.Delay(delay);
                this._content.Effect = null;
                await Task.Delay(delay);
                this._content.Effect = effect;
                await Task.Delay(delay);
                this._content.Effect = null;
                await Task.Delay(delay);
                this._content.Effect = effect;
                await Task.Delay(delay);
                this._content.Effect = null;
                await Task.Delay(delay);
                this._content.Effect = effect;
                await Task.Delay(delay);
                this._content.Effect = null;
            }
        }
    }
}
