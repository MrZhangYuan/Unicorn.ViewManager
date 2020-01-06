using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public class PopupItemContainer : Control
    {
        public PopupItem PopupItem
        {
            get
            {
                return (PopupItem)GetValue(PopupItemProperty);
            }
            set
            {
                SetValue(PopupItemProperty, value);
            }
        }
        public static readonly DependencyProperty PopupItemProperty = DependencyProperty.Register("PopupItem", typeof(PopupItem), typeof(PopupItemContainer), new PropertyMetadata(null));

        static PopupItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupItemContainer), new FrameworkPropertyMetadata(typeof(PopupItemContainer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.OnShowAnimation(null);
        }

        protected internal virtual void OnShowAnimation(Action<PopupItem> callback)
        {
            callback?.Invoke(this.PopupItem);
        }

        protected internal virtual void Flicker()
        {
            SystemSounds.Beep.Play();
        }

        protected internal virtual void OnCloseAnimation(Action<PopupItem> callback)
        {
            callback?.Invoke(this.PopupItem);
        }
    }
}
