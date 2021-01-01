using System;

namespace Unicorn.ViewManager
{
    public enum PopupItemStatus
    {
        Created,
        Showing,
        Shown,
        Closing,
        Closed
    }

    public delegate void PopupItemStatusChangedEventHandler(object sender, PopupItemStatusChangedEventArgs e);

    public class PopupItemStatusChangedEventArgs : EventArgs
    {
        public PopupItem Sender { get; }
        public PopupItemStatus OldStatus { get; }
        public PopupItemStatus NewStatus { get; }

        public PopupItemStatusChangedEventArgs(
            PopupItem sender,
            PopupItemStatus oldStatus,
            PopupItemStatus newStatus)
        {
            Sender = sender;
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}
