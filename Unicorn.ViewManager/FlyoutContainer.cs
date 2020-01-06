using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
using Unicorn.Utilities;

namespace Unicorn.ViewManager
{
    internal class FlyoutContainer : PopupItemContainer
    {
        public Flyout Flyout
        {
            get
            {
                return (Flyout)GetValue(FlyoutProperty);
            }
            set
            {
                SetValue(FlyoutProperty, value);
            }
        }
        public static readonly DependencyProperty FlyoutProperty = DependencyProperty.Register("Flyout", typeof(Flyout), typeof(FlyoutContainer), new PropertyMetadata(null));

        static FlyoutContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlyoutContainer), new FrameworkPropertyMetadata(typeof(FlyoutContainer)));
        }

        private Grid _grid = null;
        private Border _funcBorder = null;
        private Border _transformBorder = null;
        private ContentPresenter _content;
        private bool _isTemplateApplyed = false;
        public override void OnApplyTemplate()
        {
            this._grid = this.GetTemplateChild("PART_BACKGROUND") as Grid;
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
                && e.OriginalSource is Border)
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
        private double PriorityValue(params double[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!double.IsNaN(values[i])
                    && !double.IsInfinity(values[i])
                    && values[i] > 0)
                {
                    return values[i];
                }
            }
            return 0;
        }

        private bool _isCloseInvoked = false;

        protected internal override void OnShowAnimation(Action<PopupItem> callback)
        {
            this.RestoreTransform();

            if (ViewManager.Instance.ViewPreferences.UsePopupViewAnimations
                && this._isTemplateApplyed)
            {
                this._grid.Opacity = 0;
                this._grid.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.ControlOpacityToOne,
                    Values = new TransformValues(),
                    StoryboardComplateCallBack = obj =>
                    {
                        if (!this._isCloseInvoked)
                        {
                            this._grid.BeginAnimation(Control.OpacityProperty, null);
                            this._grid.Opacity = 1;
                        }

                        callback?.Invoke(this.PopupItem);
                    }
                });

                double from_x = 0,
                    from_y = 0;

                switch (this.Flyout.FlyoutLocation)
                {
                    case FlyoutLocation.Left:
                        from_y = 0;
                        from_x = -PriorityValue(this.Flyout.MinWidth, this.Flyout.Width, this.Flyout.ActualWidth);
                        from_x = Math.Max(-100, from_x);
                        break;

                    case FlyoutLocation.Right:
                        from_y = 0;
                        from_x = PriorityValue(this.Flyout.MinWidth, this.Flyout.Width, this.Flyout.ActualWidth);
                        from_x = Math.Min(100, from_x);
                        break;

                    case FlyoutLocation.Bottom:
                        from_y = PriorityValue(this.Flyout.MinHeight, this.Flyout.Height, this.Flyout.ActualHeight);
                        from_x = 0;
                        from_y = Math.Min(100, from_y);
                        break;

                    case FlyoutLocation.Top:
                        from_y = -PriorityValue(this.Flyout.MinHeight, this.Flyout.Height, this.Flyout.ActualHeight);
                        from_x = 0;
                        from_y = Math.Max(-100, from_y);
                        break;
                }

                this._transformBorder.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.TranslateTransformToValue,
                    Values = new TransformValues
                    {
                        TranslateFromX = from_x,
                        TranslateFromY = from_y,
                        TranslateToX = 0,
                        TranslateToY = 0
                    }
                });
            }
            else
            {
                callback?.Invoke(this.PopupItem);
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
                this._grid.IsHitTestVisible = false;

                this._grid.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.ControlOpacityToZero,
                    Values = new TransformValues(),
                    StoryboardComplateCallBack = obj =>
                    {
                        this._grid.BeginAnimation(Control.OpacityProperty, null);
                        this._grid.Opacity = 0;

                        base.OnCloseAnimation(callback);
                    }
                });

                double to_x = 0,
                    to_y = 0;

                switch (this.Flyout.FlyoutLocation)
                {
                    case FlyoutLocation.Left:
                        to_y = 0;
                        to_x = -PriorityValue(this.Flyout.MinWidth, this.Flyout.Width, this.Flyout.ActualWidth);
                        to_x = Math.Max(-100, to_x);
                        break;

                    case FlyoutLocation.Right:
                        to_y = 0;
                        to_x = PriorityValue(this.Flyout.MinWidth, this.Flyout.Width, this.Flyout.ActualWidth);
                        to_x = Math.Min(100, to_x);
                        break;

                    case FlyoutLocation.Bottom:
                        to_y = PriorityValue(this.Flyout.MinHeight, this.Flyout.Height, this.Flyout.ActualHeight);
                        to_x = 0;
                        to_y = Math.Min(100, to_y);
                        break;

                    case FlyoutLocation.Top:
                        to_y = -PriorityValue(this.Flyout.MinHeight, this.Flyout.Height, this.Flyout.ActualHeight);
                        to_x = 0;
                        to_y = Math.Max(-100, to_y);
                        break;
                }

                this._transformBorder.BeginTransformAnimation(new AnimationParameter
                {
                    ControlAnimation = ControlAnimation.TranslateTransformToValue,
                    Values = new TransformValues
                    {
                        TranslateFromX = 0,
                        TranslateFromY = 0,
                        TranslateToX = to_x,
                        TranslateToY = to_y
                    }
                });
            }
            else
            {
                base.OnCloseAnimation(callback);
            }
        }

        protected internal override void Flicker()
        {
            base.Flicker();
        }
    }
}
