using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Unicorn.Utilities;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{
    class WindowInfo
    {
        public Window Window { get; }
        public WindowInfo(Window window)
        {
            Window = window ?? throw new ArgumentNullException(nameof(window));
        }
    }
    static class WindowManager
    {
        internal static readonly Dictionary<IntPtr, WindowInfo> _allWindowInfos = new Dictionary<IntPtr, WindowInfo>();

        public static void RegisterWindow(Window window)
        {
            if (window != null)
            {
                window.Closed += Window_Closed;

                var handle = new WindowInteropHelper(window).Handle;
                if (handle != IntPtr.Zero)
                {
                    if (!_allWindowInfos.ContainsKey(handle))
                    {
                        _allWindowInfos.Add(handle, new WindowInfo(window));
                    }
                }
                else
                {
                    window.SourceInitialized += Window_SourceInitialized;
                }
            }
        }

        private static void Window_SourceInitialized(object sender, EventArgs e)
        {
            Window win = (Window)sender;
            win.SourceInitialized -= Window_SourceInitialized;
            var handle = new WindowInteropHelper(win).Handle;
            if (handle != IntPtr.Zero)
            {
                if (!_allWindowInfos.ContainsKey(handle))
                {
                    _allWindowInfos.Add(handle, new WindowInfo(win));
                }
            }
        }
        private static void Window_Closed(object sender, EventArgs e)
        {
            Window win = (Window)sender;
            win.Closed -= Window_Closed;
            var handle = new WindowInteropHelper(win).Handle;
            _allWindowInfos.Remove(handle);
        }
    }
    static class AutoZOrderManager
    {
        private static DispatcherTimer autoZOrderTimer;
        private static Window currentDragOverWindow;

        public static Window CurrentDragOverWindow
        {
            get => AutoZOrderManager.currentDragOverWindow;
            set
            {
                if (AutoZOrderManager.currentDragOverWindow == value)
                    return;
                AutoZOrderManager.StopTimer();
                AutoZOrderManager.currentDragOverWindow = value;
                if (AutoZOrderManager.currentDragOverWindow == null)
                    return;
                AutoZOrderManager.autoZOrderTimer = new DispatcherTimer(TimeSpan.FromMilliseconds(500), DispatcherPriority.Normal, new EventHandler(AutoZOrderManager.OnAutoZOrderTimer), AutoZOrderManager.currentDragOverWindow.Dispatcher);
                AutoZOrderManager.autoZOrderTimer.Start();
            }
        }

        public static void AdornersCleared(Window window)
        {
            if (AutoZOrderManager.CurrentDragOverWindow != window)
                return;
            AutoZOrderManager.CurrentDragOverWindow = (Window)null;
        }

        private static void OnAutoZOrderTimer(object obj, EventArgs args)
        {
            AutoZOrderManager.StopTimer();
            if (AutoZOrderManager.CurrentDragOverWindow == null)
                return;
            int num = 1;
            IntPtr draggedWindowHandle = DockManager.Instance.CurrentDraggedWindowHandle;
            IntPtr handle = new WindowInteropHelper(AutoZOrderManager.CurrentDragOverWindow).Handle;
            foreach (Window floatingWindow in WindowManager._allWindowInfos.Values.Select(_p => _p.Window))
            {
                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(floatingWindow);
                if (windowInteropHelper.Handle != draggedWindowHandle && windowInteropHelper.Owner == handle)
                    ++num;
            }

            var older = GetWindowZOrder(AutoZOrderManager.CurrentDragOverWindow);
            if (older == num)
                return;

            NativeMethods.SetWindowPos(handle, draggedWindowHandle, 0, 0, 0, 0, 16403);

            if (DockManager.Instance.CurrentDockDragGrip == null)
                return;

            DockManager.Instance.CleanupAdorners();
            DockManager.Instance.CurrentDockDragGrip.RaiseDragAbsolute(NativeMethods.GetCursorPos());
        }

        private static void StopTimer()
        {
            if (AutoZOrderManager.autoZOrderTimer == null)
                return;
            AutoZOrderManager.autoZOrderTimer.Stop();
            AutoZOrderManager.autoZOrderTimer = (DispatcherTimer)null;
        }


        internal static int GetWindowZOrder(Window window) => GetWindowZOrder(window, true);

        private static int GetWindowZOrder(Window window, bool includeMainWindow)
        {
            var handles = WindowManager._allWindowInfos.Keys;

            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            int num1 = 0;
            IntPtr num2 = !includeMainWindow ? new IntPtr(-1) : new WindowInteropHelper(Application.Current.MainWindow).Handle;
            while (hwnd != IntPtr.Zero)
            {
                hwnd = NativeMethods.GetWindow(hwnd, 3);
                if (hwnd != IntPtr.Zero && (num2 == hwnd || handles.Contains(hwnd)))
                    ++num1;
            }
            return num1;
        }

    }

    class DragContext
    {
        public DockDragGrip CurrentDockDragGrip { get; set; }
        public Window CurrentDraggedWindow { get; set; }
        public Window CurrentDragOverWindow { get; set; }

    }

    class DockManager
    {
        protected class DockSite
        {
            private Dictionary<DockDirection, DockAdornerWindow> adorners = new Dictionary<DockDirection, DockAdornerWindow>();

            public IntPtr Handle
            {
                get;
                set;
            }

            public Visual Visual
            {
                get;
                set;
            }

            public DockAdornerWindow GetAdornerLayer(DockDirection type)
            {
                if (!adorners.TryGetValue(type, out DockAdornerWindow value))
                {
                    value = new DockAdornerWindow(Handle);
                    adorners[type] = value;
                }
                return value;
            }
        }

        protected class DockSiteHitTestResult
        {
            public DockSite DockSite
            {
                get;
                private set;
            }

            public Visual VisualHit
            {
                get;
                private set;
            }

            public DockSiteHitTestResult(DockSite site, Visual visualHit)
            {
                DockSite = site;
                VisualHit = visualHit;
            }
        }

        private static DockManager _instance = null;
        public static DockManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DockManager();
                }
                return _instance;
            }
        }


        public IntPtr CurrentDraggedWindowHandle { get; set; }
        public DockDragGrip CurrentDockDragGrip { get; set; }


        private Dictionary<Visual, DockSite> visualToSite = new Dictionary<Visual, DockSite>();

        private Dictionary<IntPtr, DockSite> hwndToSite = new Dictionary<IntPtr, DockSite>();
        private DockManager()
        {

        }

        public void Init()
        {
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragStartedEvent, new EventHandler<DragAbsoluteEventArgs>(OnDockViewDragGripDragStarted));
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragAbsoluteEvent, new EventHandler<DragAbsoluteEventArgs>(OnDockViewDragGripDragAbsolute));
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragDeltaEvent, new DragDeltaEventHandler(OnDockViewDragGripDragDelta));
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragCompletedAbsoluteEvent, new EventHandler<DragAbsoluteCompletedEventArgs>(OnDockViewDragGripDragCompletedAbsolute));
        }

        private static void OnDockViewDragGripDragStarted(object sender, DragAbsoluteEventArgs e)
        {
            DockManager.Instance.CurrentDockDragGrip = sender as DockDragGrip;
        }

        private static void OnDockViewDragGripDragDelta(object sender, DragDeltaEventArgs e)
        {

        }

        private static void OnDockViewDragGripDragAbsolute(object sender, DragAbsoluteEventArgs e)
        {
            DockDragGrip dragGrip = sender as DockDragGrip;

            if (!dragGrip.IsWindowTitleBar)
            {
                DockItem dockView = dragGrip.FindAncestor<DockItem>();

                DockGroupControl dockGroupControl = dockView.FindAncestor<DockGroupControl>();
                if (dockGroupControl != null
                    && (dockGroupControl.Items.Contains(dockView) || dockGroupControl.Items.Contains(dockView.Content)))
                {
                    var undockedPosition = new Rect(dockView.PointToScreen(new Point(0.0, 0.0)), DpiHelper.LogicalToDeviceUnits(dockView.RenderSize));

                    if (dockGroupControl.Items.Contains(dockView))
                    {
                        dockGroupControl.Items.Remove(dockView);
                    }
                    else
                    {
                        dockGroupControl.Items.Remove(dockView.Content);
                    }

                    dragGrip.CancelDrag();

                    dragGrip.IsWindowTitleBar = true;

                    FloatingWindow floatwindow = new FloatingWindow()
                    {
                        Top = undockedPosition.Y,
                        Left = undockedPosition.X,
                        Height = undockedPosition.Height,
                        Width = undockedPosition.Width,
                        //Owner = old
                    };
                    DockGroupControl group = new DockGroupControl();
                    group.Items.Add(dockView);
                    floatwindow.Content = group;
                    floatwindow.Show();

                    DockManager.Instance.CurrentDraggedWindowHandle = new WindowInteropHelper(floatwindow).Handle;

                    floatwindow.DragMove();
                }
            }
            else
            {
                FloatingWindow win = FloatingWindow.GetWindow(dragGrip) as FloatingWindow;

                if (win != null)
                {
                    //停靠位置标志处理

                    //非当前拖动窗体和非DockAdornerWindow窗体并且某个上级类型为DockTarget
                    DockSiteHitTestResult hitresult = DockManager.Instance.FindValidHitElement<DockTarget>(
                        e.ScreenPoint,
                        _ds=> _ds.Visual != win 
                                && !(_ds.Visual is DockAdornerWindow),
                        out DockTarget dockTarget
                    );


                    if (hitresult != null)
                    {
                        AutoZOrderManager.CurrentDragOverWindow = Window.GetWindow(hitresult.VisualHit);// hitresult.VisualHit.FindAncestorOrSelf<Window>();

                        if (dockTarget != null)
                        {
                            DockTarget outsidedockTarget = dockTarget.FindAncestor<DockTarget>(_p => _p.DockTargetType == DockTargetType.Outside);

                            if (outsidedockTarget != null)
                            {
                                DockManager.Instance.CleanupAdorners(dockTarget, outsidedockTarget);
                            }
                            else
                            {
                                DockManager.Instance.CleanupAdorners(dockTarget);
                            }

                            if (!DockManager.Instance._adorners.ContainsKey(dockTarget))
                            {
                                if (outsidedockTarget != null
                                    && !DockManager.Instance._adorners.ContainsKey(outsidedockTarget))
                                {
                                    List<DockAdornerWindow> adolist = new List<DockAdornerWindow>();

                                    var handle = hitresult.DockSite.Handle;

                                    adolist.Add(new DockAdornerWindow(handle)
                                    {
                                        AdornedElement = outsidedockTarget,
                                        DockDirection = DockDirection.Top
                                    });
                                    adolist.Add(new DockAdornerWindow(handle)
                                    {
                                        AdornedElement = outsidedockTarget,
                                        DockDirection = DockDirection.Right
                                    });
                                    adolist.Add(new DockAdornerWindow(handle)
                                    {
                                        AdornedElement = outsidedockTarget,
                                        DockDirection = DockDirection.Bottom
                                    });
                                    adolist.Add(new DockAdornerWindow(handle)
                                    {
                                        AdornedElement = outsidedockTarget,
                                        DockDirection = DockDirection.Left
                                    });
                                    foreach (var item in adolist)
                                    {
                                        item.PrepareAndShow();
                                    }
                                    DockManager.Instance._adorners.Add(outsidedockTarget, adolist);
                                }

                                List<DockAdornerWindow> selfadolist = new List<DockAdornerWindow>();
                                selfadolist.Add(new DockAdornerWindow(hitresult.DockSite.Handle)
                                {
                                    AdornedElement = dockTarget,
                                    DockDirection = DockDirection.Fill,
                                    Orientation = Orientation.Vertical,
                                    ////AreInnerSideTargetsEnabled = true,
                                    AreInnerTargetsEnabled = true,
                                    IsInnerCenterTargetEnabled = true,
                                    ////AreOuterTargetsEnabled = true
                                });
                                foreach (var item in selfadolist)
                                {
                                    item.PrepareAndShow();
                                }

                                DockManager.Instance._adorners.Add(dockTarget, selfadolist);
                            }
                        }
                        else
                        {
                            DockManager.Instance.CleanupAdorners();
                        }
                    }
                    else
                    {
                        DockManager.Instance.CleanupAdorners();
                    }



                    //停靠预览处理
                    DockSiteHitTestResult dockSiteHitTestResult = DockManager.Instance.FindHitElements(e.ScreenPoint, (DockSite s) => s.Visual != win).FirstOrDefault();
                    if (dockSiteHitTestResult != null)
                    {
                        DockSiteAdorner dockSiteAdorner = dockSiteHitTestResult.VisualHit.FindAncestorOrSelf<DockSiteAdorner>();
                        DockAdornerWindow dockAdornerWindow = dockSiteHitTestResult.VisualHit.FindAncestorOrSelf<DockAdornerWindow>();

                        if (dockSiteAdorner != null
                            && dockAdornerWindow != null)
                        {
                            var frameworkElement = dockAdornerWindow.AdornedElement;

                            if (frameworkElement != null)
                            {
                                var dockDirection = dockSiteAdorner.DockDirection;

                                SetupDockPreviewArgs setupDockPreviewArgs = default(SetupDockPreviewArgs);
                                setupDockPreviewArgs.previewRect = DockManager.Instance.GetDockPreviewRect(dockDirection, frameworkElement, dragGrip.FindAncestor<DockItem>());
                                setupDockPreviewArgs.dockTargetType = DockTargetType.Outside;
                                setupDockPreviewArgs.screenPoint = e.ScreenPoint;
                                setupDockPreviewArgs.dockDirection = dockDirection;
                                setupDockPreviewArgs.adornedElement = frameworkElement;
                                SetupDockPreviewArgs args2 = setupDockPreviewArgs;
                                DockManager.Instance.DockPreviewWindow.SetupDockPreview(args2);
                                DockManager.Instance.DockPreviewWindow.Show(dockSiteHitTestResult.DockSite.Handle);
                            }
                            else
                            {
                                DockManager.Instance.HideDockPreview();
                            }
                        }
                        else
                        {
                            DockManager.Instance.HideDockPreview();
                        }
                    }
                    else
                    {
                        DockManager.Instance.HideDockPreview();
                    }
                }
            }
        }


        private static void OnDockViewDragGripDragCompletedAbsolute(object sender, DragAbsoluteCompletedEventArgs e)
        {
            foreach (var item in DockManager.Instance._adorners)
            {
                foreach (var awin in item.Value)
                {
                    awin.PrepareAndHide();
                }
            }

            DockManager.Instance._adorners.Clear();

            AutoZOrderManager.CurrentDragOverWindow = null;
            DockManager.Instance.CurrentDockDragGrip = sender as DockDragGrip;
        }



        private IDockPreviewWindow dockPreviewWindow;
        private IDockPreviewWindow DockPreviewWindow
        {
            get
            {
                if (dockPreviewWindow == null)
                {
                    dockPreviewWindow = new DockPreviewWindow();
                }
                return dockPreviewWindow;
            }
        }

        private void HideDockPreview()
        {
            if (dockPreviewWindow != null)
            {
                dockPreviewWindow.Hide();
            }
        }

        private Rect GetDockPreviewRect(DockDirection dockDirection, FrameworkElement docktarget, DockItem drageddockItem)
        {
            Orientation orientation = Orientation.Horizontal;
            switch (dockDirection)
            {
                case DockDirection.Top:
                case DockDirection.Bottom:
                    orientation = Orientation.Vertical;
                    break;
                case DockDirection.Left:
                case DockDirection.Right:
                    orientation = Orientation.Horizontal;
                    break;
            }

            if (dockDirection != DockDirection.Fill)
            {
                SplitterPanel panel = docktarget.FindAncestor<SplitterPanel>();
                if (panel != null
                    && orientation == panel.Orientation)
                {
                    return PreviewDockSameOrientation(dockDirection, docktarget, drageddockItem, orientation);
                }

                return PreviewDockCounterOrientation(dockDirection, docktarget, orientation);
            }

            return PreviewDockFill(docktarget);
        }

        private Rect PreviewDockSameOrientation(DockDirection dockDirection,
            FrameworkElement docktarget,
            DockItem drageddockItem,
            Orientation orientation)
        {
            //找到父SplitterPanel 和 停靠目标 SplitterItem 的索引位置
            SplitterPanel targetpanel = docktarget.FindAncestor<SplitterPanel>();
            int originalIndex = -1;
            if (targetpanel != null)
            {
                SplitterItem targetspitem = docktarget.FindAncestor<SplitterItem>();
                originalIndex = SplitterPanel.GetIndex(targetspitem);
            }

            if (dockDirection == DockDirection.Right
                || dockDirection == DockDirection.Bottom)
            {
                originalIndex++;
            }

            double length = 0;
            switch (dockDirection)
            {
                case DockDirection.Left:
                case DockDirection.Right:
                    length = targetpanel.ActualWidth / (targetpanel.ActualWidth + drageddockItem.ActualWidth) * drageddockItem.ActualWidth;
                    break;

                case DockDirection.Top:
                case DockDirection.Bottom:
                    length = targetpanel.ActualHeight / (targetpanel.ActualHeight + drageddockItem.ActualHeight) * drageddockItem.ActualHeight;
                    break;
            }

            Size spacesize = new Size(targetpanel.ActualWidth, targetpanel.ActualHeight);

            SplitterLength previewlength = new SplitterLength(length);
            SplitterItem drageditem = new SplitterItem();
            double maxlength = (orientation == Orientation.Horizontal) ? (spacesize.Width / 2.0) : (spacesize.Height / 2.0);
            SplitterPanel.SetMaximumLength(drageditem, maxlength);
            SplitterPanel.SetSplitterLength(drageditem, previewlength);

            List<UIElement> templist = targetpanel.Children.Cast<UIElement>().ToList();
            templist.Insert(originalIndex, drageditem);

            Point point = targetpanel.PointToScreen(new Point(0.0, 0.0));
            IList<SplitterMeasureData> meadatas = SplitterMeasureData.FromElements(templist);
            SplitterPanel.Measure(spacesize, orientation, meadatas, false);
            Rect result = DpiHelper.LogicalToDeviceUnits(meadatas[originalIndex].MeasuredBounds);
            result.Offset(point.X, point.Y);

            return result;
        }

        private Rect PreviewDockCounterOrientation(DockDirection dockDirection,
            FrameworkElement docktarget,
            Orientation orientation)
        {
            List<UIElement> templist = new List<UIElement>();
            SplitterItem drageditem = new SplitterItem();
            SplitterItem targetitem = new SplitterItem();

            templist.Add(targetitem);
            int index = 0;
            if (dockDirection == DockDirection.Right
                || dockDirection == DockDirection.Bottom)
            {
                index = 1;
            }
            templist.Insert(index, drageditem);

            Size spacesize = new Size(docktarget.ActualWidth, docktarget.ActualHeight);

            double length = (orientation == Orientation.Horizontal) ? (spacesize.Width / 2.0) : (spacesize.Height / 2.0);
            SplitterPanel.SetMaximumLength(drageditem, length);

            //默认均分
            //SplitterLength previewlength = new SplitterLength(length);
            //SplitterPanel.SetSplitterLength(targetitem, previewlength);
            //SplitterPanel.SetSplitterLength(drageditem, previewlength);
            Point point = docktarget.PointToScreen(new Point(0.0, 0.0));
            IList<SplitterMeasureData> meadatas = SplitterMeasureData.FromElements(templist);
            SplitterPanel.Measure(spacesize, orientation, meadatas, false);
            Rect result = DpiHelper.LogicalToDeviceUnits(meadatas[index].MeasuredBounds);
            result.Offset(point.X, point.Y);

            return result;
        }

        private Rect PreviewDockFill(FrameworkElement adornedElement)
        {
            Point location = adornedElement.PointToScreen(new Point(0.0, 0.0));
            Size size = default(Size);
            if (adornedElement.ActualHeight.IsNonreal() || adornedElement.ActualWidth.IsNonreal())
            {
                size = adornedElement.DesiredSize;
            }
            else
            {
                size.Width = adornedElement.ActualWidth;
                size.Height = adornedElement.ActualHeight;
            }
            return new Rect(location, DpiHelper.LogicalToDeviceUnits(size));
        }







        public void CleanupAdorners(params DockTarget[] targets)
        {
            var removedlist = new List<DockTarget>();

            foreach (var item in DockManager.Instance._adorners)
            {
                if (targets != null && targets.Contains(item.Key))
                {
                    continue;
                }

                removedlist.Add(item.Key);

                foreach (var awin in item.Value)
                {
                    awin.PrepareAndHide();
                }
            }

            foreach (var item in removedlist)
            {
                DockManager.Instance._adorners.Remove(item);
            }
        }

        private Dictionary<DockTarget, List<DockAdornerWindow>> _adorners = new Dictionary<DockTarget, List<DockAdornerWindow>>();





        public void RegisterDockSite(Window window)
        {
            HwndSource hwndSource = PresentationSource.FromDependencyObject(window) as HwndSource;
            if (hwndSource == null)
            {
                window.SourceInitialized += OnSourceInitialized;
            }
            else
            {
                RegisterDockSite(window, hwndSource.Handle);
            }
        }

        public void RegisterDockSite(Visual visual, IntPtr hWnd)
        {
            if (visualToSite.ContainsKey(visual))
            {
                if (visualToSite[visual].Handle != hWnd)
                {
                    throw new InvalidOperationException("Visual cannot be used in RegisterSite with two different window handles");
                }
            }
            else
            {
                DockSite value = new DockSite
                {
                    Handle = hWnd,
                    Visual = visual
                };
                visualToSite[visual] = value;
                hwndToSite[hWnd] = value;
            }
        }

        private void OnSourceInitialized(object sender, EventArgs args)
        {
            Window window = (Window)sender;
            window.SourceInitialized -= OnSourceInitialized;
            RegisterDockSite(window);
        }

        public void UnregisterDockSite(Visual visual)
        {
            if (visualToSite.TryGetValue(visual, out DockSite value))
            {
                visualToSite.Remove(visual);
                hwndToSite.Remove(value.Handle);
            }
        }



        private DockSiteHitTestResult FindValidHitElement<T>(Point point, Predicate<DockSite> predicate, out T target) where T : DependencyObject
        {
            target = null;

            foreach (DockSiteHitTestResult item in FindHitElements(point, predicate))
            {
                for (DependencyObject deobj = item.VisualHit; deobj != null; deobj = deobj.GetVisualOrLogicalParent())
                {
                    target = deobj as T;
                    if (target != null)
                    {
                        return item;
                    }
                }
            }

            return null;
        }


        private List<DockSiteHitTestResult> FindHitElements(Point point, Predicate<DockSite> predicate)
        {
            List<DockSite> sortedDockSites = GetSortedDockSites();
            List<DockSiteHitTestResult> results = new List<DockSiteHitTestResult>();
            foreach (DockSite item in from _p in sortedDockSites
                                      where predicate(_p)
                                      select _p)
            {
                if (item.Visual.IsConnectedToPresentationSource())
                {
                    ViewTreeHelper.HitTestVisibleElements(
                        item.Visual,
                        delegate (HitTestResult result)
                        {
                            Visual visualHit = (Visual)result.VisualHit;
                            results.Add(new DockSiteHitTestResult(item, visualHit));
                            return HitTestResultBehavior.Stop;
                        },
                        new PointHitTestParameters(item.Visual.PointFromScreen(point))
                    );
                }
            }
            return results;
        }

        private List<DockSite> GetSortedDockSites()
        {
            void AddDockSiteFromHwnd(List<DockSite> sortedSites, IntPtr hWnd)
            {
                if (hwndToSite.TryGetValue(hWnd, out DockSite value))
                {
                    sortedSites.Add(value);
                }
            }

            List<DockSite> sortedSites = new List<DockSite>();
            NativeMethods.EnumThreadWindows(
                NativeMethods.GetCurrentThreadId(),
                delegate (IntPtr hWnd, IntPtr lParam)
                {
                    NativeMethods.EnumChildWindows(
                        hWnd,
                        delegate (IntPtr hWndChild, IntPtr lParamChild)
                        {
                            AddDockSiteFromHwnd(sortedSites, hWndChild);
                            return true;
                        },
                        IntPtr.Zero
                    );
                    AddDockSiteFromHwnd(sortedSites, hWnd);
                    return true;
                },
                IntPtr.Zero
            );

            return sortedSites;
        }
    }
}
