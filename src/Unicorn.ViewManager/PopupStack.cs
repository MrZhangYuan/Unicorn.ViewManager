using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
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
            if (popupStackControl == null)
            {
                throw new ArgumentNullException(nameof(popupStackControl));
            }

            this._parentPopupStackControl = popupStackControl;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PopupItemContainer;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            this._parentPopupStackControl.OnViewStackChanged(
                new ViewStackChangedEventArgs(
                    (ViewStackAction)(int)(e.Action),
                    e.NewItems?.OfType<PopupItem>()?.ToList(),
                    e.NewStartingIndex,
                    e.OldItems?.OfType<PopupItem>()?.ToList(),
                    e.OldStartingIndex
                ));
        }
    }
}
