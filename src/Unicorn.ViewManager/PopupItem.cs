using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Unicorn.ViewManager
{
    [TemplatePart(Name = PART_POPUPSTACK, Type = typeof(ContentPresenter))]
    public abstract class PopupItem : ContentControl, IPopupItemContainer
    {
        const string PART_POPUPSTACK = "PART_POPUPSTACK";

        private static readonly object EVENT_CLOSING = new object();
        private static readonly object EVENT_CLOSED = new object();
        private static readonly object EVENT_SHOWING = new object();
        private static readonly object EVENT_SHOWN = new object();

        internal bool _isHostAtViewStack = false;
        internal bool _isClosing = false;
        internal bool _isShowing = false;
        internal bool _showingAsModal = false;
        internal DispatcherFrame _dispatcherFrame = null;
        internal ModalResult _modalResult;
        private EventHandlerList _events;
        private readonly PopupStackControl _popupStackControl = null;
        private IPopupItemContainer _parentHostContainer = null;
        private PopupStackControl _parentHostStack = null;

        public IPopupItemContainer ParentHostContainer
        {
            get
            {
                if (this.ParentHostStack != null)
                {
                    return this.ParentHostStack;
                }

                return _parentHostContainer;
            }
        }

        internal PopupStackControl PopupStackControl
        {
            get
            {
                return this._popupStackControl;
            }
        }

        public ModalResult ModalResult
        {
            get
            {
                return _modalResult;
            }
            set
            {
                if (!this._showingAsModal)
                {
                    throw new InvalidOperationException($"当前 {this.GetType()} 不作为模态显示，因此不能设置ModalResult");
                }
                _modalResult = value;
                this.Close();
            }
        }

        private EventHandlerList Events
        {
            get
            {
                if (this._events == null)
                {
                    this._events = new EventHandlerList();
                }
                return this._events;
            }
        }

        public IEnumerable<PopupItem> Children
        {
            get
            {
                return this._popupStackControl.Items;
            }
        }

        public PopupItem ParentPopup
        {
            get;
            internal set;
        }

        public bool IsEasyClose
        {
            get
            {
                return (bool)GetValue(IsEasyCloseProperty);
            }
            set
            {
                SetValue(IsEasyCloseProperty, value);
            }
        }
        public static readonly DependencyProperty IsEasyCloseProperty = DependencyProperty.Register("IsEasyClose", typeof(bool), typeof(PopupItem), new PropertyMetadata(false));


        public event CancelEventHandler Showing
        {
            add
            {
                this.Events.AddHandler(PopupItem.EVENT_SHOWING, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(PopupItem.EVENT_SHOWING, (Delegate)value);
            }
        }
        public event EventHandler Shown
        {
            add
            {
                this.Events.AddHandler(PopupItem.EVENT_SHOWN, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(PopupItem.EVENT_SHOWN, (Delegate)value);
            }
        }

        public event CancelEventHandler Closing
        {
            add
            {
                this.Events.AddHandler(PopupItem.EVENT_CLOSING, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(PopupItem.EVENT_CLOSING, (Delegate)value);
            }
        }

        public event EventHandler Closed
        {
            add
            {
                this.Events.AddHandler(PopupItem.EVENT_CLOSED, (Delegate)value);
            }
            remove
            {
                this.Events.RemoveHandler(PopupItem.EVENT_CLOSED, (Delegate)value);
            }
        }

        public PopupItem()
        {
            this._popupStackControl = new PopupStackControl(this);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var presenter = this.GetTemplateChild(PART_POPUPSTACK) as ContentPresenter;

            if (presenter != null)
            {
                presenter.Content = this._popupStackControl;
            }
        }

        internal void InternalShowing(out CancelEventArgs e)
        {
            e = new CancelEventArgs(false);
            this.OnShowing(e);
        }

        protected virtual void OnShowing(CancelEventArgs e)
        {
            CancelEventHandler cancelEventHandler = (CancelEventHandler)this.Events[PopupItem.EVENT_SHOWING];
            if (cancelEventHandler == null)
                return;
            cancelEventHandler((object)this, e);
        }

        internal void InternalShown(out EventArgs e)
        {
            e = new EventArgs();
            this.OnShown(e);
        }

        protected virtual void OnShown(EventArgs e)
        {
            EventHandler eventHandler = (EventHandler)this.Events[PopupItem.EVENT_SHOWN];
            if (eventHandler == null)
                return;
            eventHandler((object)this, e);
        }

        internal void InternalClosing(out CancelEventArgs e)
        {
            e = new CancelEventArgs(false);
            this.OnClosing(e);
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            CancelEventHandler cancelEventHandler = (CancelEventHandler)this.Events[PopupItem.EVENT_CLOSING];
            if (cancelEventHandler == null)
                return;
            cancelEventHandler((object)this, e);
        }

        internal void InternalClosed(out EventArgs e)
        {
            e = new EventArgs();

            this.CloseChildren();

            this.OnClosed(e);
        }

        private void CloseChildren()
        {
            foreach (var item in this.Children)
            {
                item.Close();
            }
        }

        protected virtual void OnClosed(EventArgs e)
        {
            EventHandler eventHandler = (EventHandler)this.Events[PopupItem.EVENT_CLOSED];
            if (eventHandler == null)
                return;
            eventHandler((object)this, e);
        }

        internal PopupStackControl ParentHostStack
        {
            get
            {
                return this._parentHostStack;
            }
            set
            {
                this._parentHostStack = value;
                this._parentHostContainer = value;
            }
        }



        #region Show Self

        public void Show()
        {
            ViewManager.Instance.MainRichView.Show(this);
        }

        public void Show(IPopupItemContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var oldcontainer = this._parentHostContainer;
            this._parentHostContainer = container;
            try
            {
                container.Show(this);
            }
            catch (Exception)
            {
                this._parentHostContainer = oldcontainer;
                throw;
            }
        }

        public ModalResult ShowAsModal()
        {
            return ViewManager.Instance.MainRichView.ShowModal(this);
        }
        public ModalResult ShowAsModal(IPopupItemContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            var oldcontainer = this._parentHostContainer;           
            this._parentHostContainer = container;
            try
            {
                return container.ShowModal(this);
            }
            catch (Exception)
            {
                this._parentHostContainer = oldcontainer;
                throw;
            }
        }

        public void Close()
        {
            if (this.ParentHostStack != null)
            {
                this.ParentHostStack.Close(this);
            }
            else
            {
                ViewManager.Instance.MainRichView.Close(this);
            }
        }

        #endregion


        #region Show Child
        public ModalResult ShowModal(PopupItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this._popupStackControl.ShowModal(item);
        }

        public void Show(PopupItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this._popupStackControl.Show(item);
        }

        public void Close(PopupItem item)
        {
            this._popupStackControl.Close(item);
        }

        #endregion

        internal void InternalDiapose()
        {
            this.ParentHostStack = null;
            this._isClosing = false;
            this._isHostAtViewStack = false;
            this.ParentPopup = null;

            if (this._dispatcherFrame != null)
            {
                this._dispatcherFrame.Continue = false;
            }

            this._dispatcherFrame = null;
            this._showingAsModal = false;
        }

        protected internal abstract PopupItemContainer GetContainer();
    }
}
