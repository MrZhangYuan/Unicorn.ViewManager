namespace Unicorn.ViewManager
{
    public interface IPopupItemContainer
    {
        ModalResult ShowModal(PopupItem item);
        void Show(PopupItem item);
        void Close(PopupItem item);
    }
}
