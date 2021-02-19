using System.Windows;

namespace Unicorn.ViewManager
{
    public interface IResizable
    {
        Size MinSize
        {
            get;
        }
        Size MaxSize
        {
            get;
        }
        Rect CurrentScreenBounds
        {
            get;
        }
        Rect CurrentBounds
        {
            get;
        }
        void UpdateBounds(double leftDelta, double topDelta, double widthDelta, double heightDelta);
    }
}
