using System.Collections;
using System.Collections.Generic;

namespace Unicorn.ViewManager
{
    public interface IPopupItemContainer
    {
        IPopupItemContainer Parent
        {
            get;
        }

        PopupItem TopItem
        {
            get;
        }

        IEnumerable<PopupItem> Children
        {
            get;
        }

        ModalResult ShowModal(PopupItem item);

        void Show(PopupItem item);

        void Close(PopupItem item);

        bool Close();
    }
}
