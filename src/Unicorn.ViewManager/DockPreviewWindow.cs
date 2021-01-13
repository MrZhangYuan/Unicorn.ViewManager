using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    public interface IDockPreviewWindow
    {
        IntPtr Handle { get; }

        void Show(IntPtr owner);

        void Hide();

        void Close();

        void SetupDockPreview(SetupDockPreviewArgs args);

        int InsertPosition { get; }
    }

    public struct SetupDockPreviewArgs
    {
        public Rect previewRect;
        public DockTargetType dockTargetType;
        public Point screenPoint;
        public DockDirection dockDirection;
        public FrameworkElement adornedElement;
    }

    public sealed class DockPreviewWindow : ContentControl, IDockPreviewWindow
    {
        internal const double DefaultTabHeight = 25.0;
        internal const double DefaultTabWidth = 100.0;

        private Point _screenPoint;
        private DockTargetType _dockTargetType;
        private int _floatingViewCount;


        public static readonly DependencyProperty DeviceLeftProperty;
        public static readonly DependencyProperty DeviceTopProperty;
        public static readonly DependencyProperty DeviceWidthProperty;
        public static readonly DependencyProperty DeviceHeightProperty;


        public IntPtr Handle
        {
            get => hwndWrapper == null ? IntPtr.Zero : hwndWrapper.Handle;
        }

        private HwndSource hwndWrapper;

        public double DeviceTop
        {
            get
            {
                return (double)GetValue(DeviceTopProperty);
            }
            set
            {
                SetValue(DeviceTopProperty, value);
            }
        }

        public double DeviceLeft
        {
            get
            {
                return (double)GetValue(DeviceLeftProperty);
            }
            set
            {
                SetValue(DeviceLeftProperty, value);
            }
        }

        public double DeviceWidth
        {
            get
            {
                return (double)GetValue(DeviceWidthProperty);
            }
            private set
            {
                SetValue(DeviceWidthProperty, value);
            }
        }

        public double DeviceHeight
        {
            get
            {
                return (double)GetValue(DeviceHeightProperty);
            }
            private set
            {
                SetValue(DeviceHeightProperty, value);
            }
        }

        private bool IsChanged
        {
            get;
            set;
        }

        public int InsertPosition
        {
            get;
            private set;
        }

        static DockPreviewWindow()
        {
            DeviceLeftProperty = DependencyProperty.Register("DeviceLeft", typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
            DeviceTopProperty = DependencyProperty.Register("DeviceTop", typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
            DeviceWidthProperty = DependencyProperty.Register("DeviceWidth", typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnDeviceWidthChanged));
            DeviceHeightProperty = DependencyProperty.Register("DeviceHeight", typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnDeviceHeightChanged));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPreviewWindow), new FrameworkPropertyMetadata(typeof(DockPreviewWindow)));
            FrameworkElement.WidthProperty.OverrideMetadata(typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
            FrameworkElement.HeightProperty.OverrideMetadata(typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
        }

        private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DockPreviewWindow dockPreviewWindow = obj as DockPreviewWindow;
            dockPreviewWindow.IsChanged = true;
        }

        private static void OnDeviceWidthChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DockPreviewWindow dockPreviewWindow = obj as DockPreviewWindow;
            dockPreviewWindow.Width = DpiHelper.DeviceToLogicalUnitsScalingFactorX * (double)args.NewValue;
            dockPreviewWindow.IsChanged = true;
        }

        private static void OnDeviceHeightChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DockPreviewWindow dockPreviewWindow = obj as DockPreviewWindow;
            dockPreviewWindow.Height = DpiHelper.DeviceToLogicalUnitsScalingFactorY * (double)args.NewValue;
            dockPreviewWindow.IsChanged = true;
        }

        private void CreateWindow(IntPtr owner)
        {
            HwndSourceParameters parameters = new HwndSourceParameters("DockPreviewWindow");
            int windowStyle = -2013265880;
            parameters.Width = (int)DeviceWidth;
            parameters.Height = (int)DeviceHeight;
            parameters.PositionX = (int)DeviceLeft;
            parameters.PositionY = (int)DeviceTop;
            parameters.WindowStyle = windowStyle;
            parameters.ParentWindow = owner;
            parameters.UsesPerPixelOpacity = true;
            hwndWrapper = new HwndSource(parameters);
            hwndWrapper.ContentRendered += OnContentRendered;
            hwndWrapper.SizeToContent = SizeToContent.Manual;
            hwndWrapper.RootVisual = this;
            InsertPosition = -1;
            IsChanged = false;
        }

        public void Show(IntPtr owner)
        {
            if (IsChanged)
            {
                if (hwndWrapper != null)
                {
                    Hide();
                }
                CreateWindow(owner);
                NativeMethods.SetWindowPos(hwndWrapper.Handle, IntPtr.Zero, (int)DeviceLeft, (int)DeviceTop, (int)DeviceWidth, (int)DeviceHeight, 84);
            }
        }

        public void Hide()
        {
            if (hwndWrapper != null)
            {
                hwndWrapper.Dispose();
                hwndWrapper = null;
            }
            IsChanged = true;
            InsertPosition = -1;
        }

        public void Close()
        {
            Hide();
        }

        public void SetupDockPreview(SetupDockPreviewArgs args)
        {
            DeviceLeft = args.previewRect.Left;
            DeviceTop = args.previewRect.Top;
            DeviceWidth = args.previewRect.Width;
            DeviceHeight = args.previewRect.Height;
            _screenPoint = args.screenPoint;
            _dockTargetType = args.dockTargetType;

            //if (DockTargetType.InsertTabPreview == dockTargetType && tabInfo != null)
            //{
            //    if (InsertPosition != insertPosition)
            //    {
            //        MeasurePreviewTab();
            //    }
            //}
            //if (args.dockDirection == DockDirection.Fill)
            //{
            //    GroupControl groupControl = ((Visual)args.adornedElement).FindAncestor<GroupControl>();
            //}
        }

        //private void MeasurePreviewTab()
        //{
        //}

        //private void CalculateInsertAndTabPosition(out int insertPosition, out double tabPosition)
        //{
        //}

        //private void OnPanelLayoutUpdated(object sender)
        //{
        //}

        private void OnContentRendered(object sender, EventArgs e)
        {
            //MeasurePreviewTab();
        }
    }
}
