using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
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
        public TabGroupTabItem draggedTabItem;
    }

    public sealed class DockPreviewWindow : ContentControl, IDockPreviewWindow
    {
        internal const double DefaultTabHeight = 25.0;
        internal const double DefaultTabWidth = 100.0;

        private Point _screenPoint;
        private DockTargetType _dockTargetType;
        private int _floatingViewCount;
        private IntPtr _ownerHandle;

        public static readonly DependencyProperty DeviceLeftProperty;
        public static readonly DependencyProperty DeviceTopProperty;
        public static readonly DependencyProperty DeviceWidthProperty;
        public static readonly DependencyProperty DeviceHeightProperty;
        public static readonly DependencyProperty PreviewDockTargetTypeProperty;
        public static readonly DependencyProperty PreviewDockDirectionProperty;
        public static readonly DependencyProperty PreviewIsTabInsertProperty;
        public static readonly DependencyProperty InsertMarkerLeftProperty;
        public static readonly DependencyProperty InsertMarkerTopProperty;
        public static readonly DependencyProperty InsertMarkerWidthProperty;
        public static readonly DependencyProperty InsertMarkerHeightProperty;

        public IntPtr Handle
        {
            get => hwndWrapper == null ? IntPtr.Zero : hwndWrapper.Handle;
        }

        private HwndSource hwndWrapper;

        public double DeviceTop
        {
            get => (double)GetValue(DeviceTopProperty);
            set => SetValue(DeviceTopProperty, value);
        }

        public double DeviceLeft
        {
            get => (double)GetValue(DeviceLeftProperty);
            set => SetValue(DeviceLeftProperty, value);
        }

        public double DeviceWidth
        {
            get => (double)GetValue(DeviceWidthProperty);
            private set => SetValue(DeviceWidthProperty, value);
        }

        public double DeviceHeight
        {
            get => (double)GetValue(DeviceHeightProperty);
            private set => SetValue(DeviceHeightProperty, value);
        }

        public DockTargetType PreviewDockTargetType
        {
            get => (DockTargetType)GetValue(PreviewDockTargetTypeProperty);
            private set => SetValue(PreviewDockTargetTypeProperty, value);
        }

        public DockDirection PreviewDockDirection
        {
            get => (DockDirection)GetValue(PreviewDockDirectionProperty);
            private set => SetValue(PreviewDockDirectionProperty, value);
        }

        public bool PreviewIsTabInsert
        {
            get => (bool)GetValue(PreviewIsTabInsertProperty);
            private set => SetValue(PreviewIsTabInsertProperty, value);
        }

        public double InsertMarkerLeft
        {
            get => (double)GetValue(InsertMarkerLeftProperty);
            private set => SetValue(InsertMarkerLeftProperty, value);
        }

        public double InsertMarkerTop
        {
            get => (double)GetValue(InsertMarkerTopProperty);
            private set => SetValue(InsertMarkerTopProperty, value);
        }

        public double InsertMarkerWidth
        {
            get => (double)GetValue(InsertMarkerWidthProperty);
            private set => SetValue(InsertMarkerWidthProperty, value);
        }

        public double InsertMarkerHeight
        {
            get => (double)GetValue(InsertMarkerHeightProperty);
            private set => SetValue(InsertMarkerHeightProperty, value);
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
            DeviceLeftProperty = DependencyProperty.Register(nameof(DeviceLeft), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
            DeviceTopProperty = DependencyProperty.Register(nameof(DeviceTop), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
            DeviceWidthProperty = DependencyProperty.Register(nameof(DeviceWidth), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnDeviceWidthChanged));
            DeviceHeightProperty = DependencyProperty.Register(nameof(DeviceHeight), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnDeviceHeightChanged));
            PreviewDockTargetTypeProperty = DependencyProperty.Register(nameof(PreviewDockTargetType), typeof(DockTargetType), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(DockTargetType.Center, OnPropertyChanged));
            PreviewDockDirectionProperty = DependencyProperty.Register(nameof(PreviewDockDirection), typeof(DockDirection), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(DockDirection.Fill, OnPropertyChanged));
            PreviewIsTabInsertProperty = DependencyProperty.Register(nameof(PreviewIsTabInsert), typeof(bool), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(false, OnPropertyChanged));
            InsertMarkerLeftProperty = DependencyProperty.Register(nameof(InsertMarkerLeft), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnPropertyChanged));
            InsertMarkerTopProperty = DependencyProperty.Register(nameof(InsertMarkerTop), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnPropertyChanged));
            InsertMarkerWidthProperty = DependencyProperty.Register(nameof(InsertMarkerWidth), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnPropertyChanged));
            InsertMarkerHeightProperty = DependencyProperty.Register(nameof(InsertMarkerHeight), typeof(double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(0d, OnPropertyChanged));
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
            _ownerHandle = owner;
            IsChanged = false;
        }

        public void Show(IntPtr owner)
        {
            if (hwndWrapper == null
                || _ownerHandle != owner)
            {
                DisposeWindow();
                CreateWindow(owner);
            }

            NativeMethods.SetWindowPos(hwndWrapper.Handle, IntPtr.Zero, (int)DeviceLeft, (int)DeviceTop, (int)DeviceWidth, (int)DeviceHeight, 84);
            IsChanged = false;
        }

        public void Hide()
        {
            DisposeWindow();
            IsChanged = true;
            ResetPreviewState();
        }

        public void Close()
        {
            Hide();
        }

        public void SetupDockPreview(SetupDockPreviewArgs args)
        {
            Rect previewRect = args.previewRect;

            _screenPoint = args.screenPoint;
            _dockTargetType = args.dockTargetType;
            PreviewDockTargetType = args.dockTargetType;
            PreviewDockDirection = args.dockDirection;
            ResetPreviewState();

            if (TryGetTabInsertPreview(args, out Rect insertMarkerRect, out int insertPosition))
            {
                InsertPosition = insertPosition;
                PreviewIsTabInsert = true;
                SetInsertMarker(insertMarkerRect, previewRect);
            }

            DeviceLeft = previewRect.Left;
            DeviceTop = previewRect.Top;
            DeviceWidth = previewRect.Width;
            DeviceHeight = previewRect.Height;
        }

        private void DisposeWindow()
        {
            if (hwndWrapper != null)
            {
                hwndWrapper.Dispose();
                hwndWrapper = null;
            }

            _ownerHandle = IntPtr.Zero;
        }

        private void ResetPreviewState()
        {
            InsertPosition = -1;
            PreviewIsTabInsert = false;
            InsertMarkerLeft = 0.0;
            InsertMarkerTop = 0.0;
            InsertMarkerWidth = 0.0;
            InsertMarkerHeight = 0.0;
        }

        private void SetInsertMarker(Rect insertMarkerRect, Rect previewRect)
        {
            InsertMarkerLeft = DeviceToLogicalX(insertMarkerRect.Left - previewRect.Left);
            InsertMarkerTop = DeviceToLogicalY(insertMarkerRect.Top - previewRect.Top);
            InsertMarkerWidth = DeviceToLogicalX(insertMarkerRect.Width);
            InsertMarkerHeight = DeviceToLogicalY(insertMarkerRect.Height);
        }

        private static double DeviceToLogicalX(double value)
        {
            return value * DpiHelper.DeviceToLogicalUnitsScalingFactorX;
        }

        private static double DeviceToLogicalY(double value)
        {
            return value * DpiHelper.DeviceToLogicalUnitsScalingFactorY;
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
        }

        private static bool TryGetTabInsertPreview(SetupDockPreviewArgs args, out Rect insertMarkerRect, out int insertPosition)
        {
            insertMarkerRect = default;
            insertPosition = -1;

            if (args.dockDirection != DockDirection.Fill
                || args.adornedElement == null
                || !(args.adornedElement is Visual adornedVisual))
            {
                return false;
            }

            TabGroupControl tabGroup = adornedVisual.FindAncestor<TabGroupControl>();
            if (tabGroup == null)
            {
                return false;
            }

            List<Rect> tabBounds = GetVisibleTabBounds(tabGroup);
            if (tabBounds.Count == 0)
            {
                return false;
            }

            Rect headerBounds = tabBounds[0];
            for (int i = 1; i < tabBounds.Count; i++)
            {
                headerBounds.Union(tabBounds[i]);
            }

            Rect hitBounds = headerBounds;
            hitBounds.Inflate(6.0, 4.0);
            if (!hitBounds.Contains(args.screenPoint))
            {
                return false;
            }

            insertPosition = tabBounds.Count;
            for (int i = 0; i < tabBounds.Count; i++)
            {
                Rect bounds = tabBounds[i];
                if (args.screenPoint.X < bounds.Left + bounds.Width / 2.0)
                {
                    insertPosition = i;
                    break;
                }
            }

            double boundaryX = insertPosition <= 0
                ? tabBounds[0].Left
                : insertPosition >= tabBounds.Count
                    ? tabBounds[tabBounds.Count - 1].Right
                    : tabBounds[insertPosition].Left;

            double previewWidth = GetDraggedTabPreviewWidth(args.draggedTabItem, tabBounds);
            double previewHeight = args.draggedTabItem != null && args.draggedTabItem.ActualHeight > 0
                ? DpiHelper.LogicalToDeviceUnits(args.draggedTabItem.RenderSize).Height
                : headerBounds.Height;

            double previewLeft = boundaryX - previewWidth / 2.0;
            double minLeft = headerBounds.Left;
            double maxLeft = Math.Max(headerBounds.Left, headerBounds.Right - previewWidth);
            previewLeft = Math.Max(minLeft, Math.Min(maxLeft, previewLeft));

            insertMarkerRect = new Rect(previewLeft, headerBounds.Top, previewWidth, previewHeight);
            return true;
        }

        private static double GetDraggedTabPreviewWidth(TabGroupTabItem draggedTabItem, List<Rect> tabBounds)
        {
            if (draggedTabItem != null
                && draggedTabItem.ActualWidth > 0)
            {
                return DpiHelper.LogicalToDeviceUnits(draggedTabItem.RenderSize).Width;
            }

            if (tabBounds != null
                && tabBounds.Count > 0)
            {
                return tabBounds[0].Width;
            }

            return DpiHelper.LogicalToDeviceUnits(new Size(DefaultTabWidth, DefaultTabHeight)).Width;
        }

        private static List<Rect> GetVisibleTabBounds(TabGroupControl tabGroup)
        {
            List<Rect> tabBounds = new List<Rect>();

            for (int i = 0; i < tabGroup.Items.Count; i++)
            {
                TabGroupTabItem tabItem = tabGroup.ItemContainerGenerator.ContainerFromIndex(i) as TabGroupTabItem;
                if (tabItem == null
                    || !tabItem.IsVisible
                    || tabItem.ActualWidth <= 0
                    || tabItem.ActualHeight <= 0)
                {
                    continue;
                }

                Point screenPoint = tabItem.PointToScreen(new Point(0.0, 0.0));
                Size deviceSize = DpiHelper.LogicalToDeviceUnits(tabItem.RenderSize);
                tabBounds.Add(new Rect(screenPoint, deviceSize));
            }

            return tabBounds;
        }
    }
}
