using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class HwndContentControl : HwndHost
    {
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(HwndContentControl), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(HwndContentControl.OnContentChanged)));

        public HwndSource HwndSource
        {
            get;
            private set;
        }

        protected ContentPresenter HwndSourcePresenter
        {
            get;
            private set;
        }

        public HwndContentControl()
        {
            this.HwndSourcePresenter = new ContentPresenter();
            PresentationSource.AddSourceChangedHandler((IInputElement)this, new SourceChangedEventHandler(this.OnSourceChanged));
        }

        public object Content
        {
            get => this.GetValue(HwndContentControl.ContentProperty);
            set => this.SetValue(HwndContentControl.ContentProperty, value);
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            this.HwndSource = this.CreateHwndSource(hwndParent);
            return new HandleRef((object)this, this.HwndSource.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (this.HwndSource == null)
                return;
            this.HwndSource.Dispose();
            this.HwndSource = null;
        }

        private static void OnContentChanged(
          DependencyObject obj,
          DependencyPropertyChangedEventArgs args)
        {
            HwndContentControl hwndContentControl = (HwndContentControl)obj;
            hwndContentControl.HwndSourcePresenter.Content = args.NewValue;
        }

        protected override IntPtr WndProc(
          IntPtr hwnd,
          int msg,
          IntPtr wParam,
          IntPtr lParam,
          ref bool handled)
        {
            if (msg != 61)
                return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
            handled = true;
            return IntPtr.Zero;
        }

        private HwndSource CreateHwndSource(HandleRef parent)
        {
            HwndSource hwndSource = new HwndSource(new HwndSourceParameters()
            {
                Width = 0,
                Height = 0,
                WindowStyle = 1174405120,
                ParentWindow = parent.Handle
            });
            hwndSource.RootVisual = (Visual)this.HwndSourcePresenter;
            this.AddLogicalChild((object)this.HwndSourcePresenter);
            NativeMethods.BringWindowToTop(hwndSource.Handle);
            return hwndSource;
        }

        private void OnSourceChanged(object sender, SourceChangedEventArgs e)
        {
            if (e.NewSource == null)
                this.HwndSourcePresenter.Visibility = Visibility.Collapsed;
            else
                this.HwndSourcePresenter.Visibility = Visibility.Visible;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.HwndSource != null)
                this.HwndSource.Dispose();
            base.Dispose(disposing);
        }
    }
}
