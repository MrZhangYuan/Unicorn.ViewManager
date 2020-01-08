using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Unicorn.ViewManager
{
    internal class PopupStack : ItemsControl
    {
        static PopupStack()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupStack), new FrameworkPropertyMetadata(typeof(PopupStack)));
        }

        private readonly PopupStackControl _parentPopupStackControl = null;

        public PopupStack(PopupStackControl popupStackControl)
        {
            if (popupStackControl==null)
            {
                throw new ArgumentNullException(nameof(popupStackControl));
            }

            this._parentPopupStackControl = popupStackControl;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PopupItemContainer;
        }
    }
}
