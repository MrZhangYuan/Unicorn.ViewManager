using System;
using System.Windows;

namespace Unicorn.ViewManager.Internal
{
    [Serializable]
    internal struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        public RECT(Rect rect)
        {
            this.Left = (int)rect.Left;
            this.Top = (int)rect.Top;
            this.Right = (int)rect.Right;
            this.Bottom = (int)rect.Bottom;
        }

        public void Offset(int dx, int dy)
        {
            this.Left += dx;
            this.Right += dx;
            this.Top += dy;
            this.Bottom += dy;
        }

        public Point Position => new Point((double)this.Left, (double)this.Top);

        public Size Size => new Size((double)this.Width, (double)this.Height);

        public int Height
        {
            get => this.Bottom - this.Top;
            set => this.Bottom = this.Top + value;
        }

        public int Width
        {
            get => this.Right - this.Left;
            set => this.Right = this.Left + value;
        }

        public Int32Rect ToInt32Rect() => new Int32Rect(this.Left, this.Top, this.Width, this.Height);
    }
}
