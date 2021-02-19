using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public class CustomItemsControl : ItemsControl
    {
        private readonly object _eventKey = new object();
        private readonly EventHandlerList _events = new EventHandlerList();

        public event NotifyCollectionChangedEventHandler ItemsChanged
        {
            add
            {
                this._events.AddHandler(this._eventKey, value);
            }
            remove
            {
                this._events.RemoveHandler(this._eventKey, value);
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            ((NotifyCollectionChangedEventHandler)this._events[this._eventKey])?.Invoke(this, e);
        }
    }
}
