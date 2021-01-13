﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using Unicorn.Utilities;
using Unicorn.ViewManager.Internal;

namespace Unicorn.ViewManager
{

    class DragContext : IDisposable
    {
        public Dictionary<DockTarget, List<DockAdornerWindow>> Adorners
        {
            get;
        }
        public DockDragGrip DockDragGrip
        {
            get;
            set;
        }
        public Window DraggedWindow
        {
            get;
            set;
        }

        private Window _dragOverWindow;
        public Window DragOverWindow
        {
            get => _dragOverWindow;
            set
            {
                _dragOverWindow = value;
                AutoZOrderManager.CurrentDragOverWindow = _dragOverWindow;
            }
        }

        public DockSiteAdorner HitDockSiteAdorner
        {
            get;
            set;
        }
        public DockTarget HitDockTarget
        {
            get;
            set;
        }

        private IDockPreviewWindow _dockPreviewWindow;
        public IDockPreviewWindow DockPreviewWindow
        {
            get => _dockPreviewWindow ?? (_dockPreviewWindow = new DockPreviewWindow());
        }

        public DragContext()
        {
            this.Adorners = new Dictionary<DockTarget, List<DockAdornerWindow>>();
        }

        public void ReSet()
        {

        }

        public void CleanupAdorners(List<DockTarget> targets)
        {
            var removedlist = new List<DockTarget>();

            foreach (var item in this.Adorners)
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
                this.Adorners.Remove(item);
            }
        }

        public void HideDockPreview()
        {
            if (_dockPreviewWindow != null)
            {
                _dockPreviewWindow.Hide();
            }
        }

        public void Dispose()
        {
            this.DockDragGrip = null;
            this.DraggedWindow = null;
            this.DragOverWindow = null;
            this.HitDockSiteAdorner = null;

            this.CleanupAdorners(null);
            this.HideDockPreview();
        }
    }

    static class DockManager
    {
        protected class DockSite
        {
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

        private static Dictionary<Visual, DockSite> _visualToSite = new Dictionary<Visual, DockSite>();

        private static Dictionary<IntPtr, DockSite> _hwndToSite = new Dictionary<IntPtr, DockSite>();

        public static DragContext CurrentDraggedContext { get; } = new DragContext();

        static DockManager()
        {
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragStartedEvent, new EventHandler<DragAbsoluteEventArgs>(OnDockViewDragGripDragStarted));
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragAbsoluteEvent, new EventHandler<DragAbsoluteEventArgs>(OnDockViewDragGripDragAbsolute));
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragDeltaEvent, new DragDeltaEventHandler(OnDockViewDragGripDragDelta));
            EventManager.RegisterClassHandler(typeof(DockDragGrip), DockDragGrip.DragCompletedAbsoluteEvent, new EventHandler<DragAbsoluteCompletedEventArgs>(OnDockViewDragGripDragCompletedAbsolute));
        }

        private static void OnDockViewDragGripDragStarted(object sender, DragAbsoluteEventArgs e)
        {
            DockManager.CurrentDraggedContext.DockDragGrip = sender as DockDragGrip;
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

                var currenthost = (IDockHost)dockView.FindAncestor<DependencyObject>(_p => _p is IDockHost);


                if (currenthost != null)
                {
                    var undockedPosition = new Rect(dockView.PointToScreen(new Point(0.0, 0.0)), DpiHelper.LogicalToDeviceUnits(dockView.RenderSize));


                    currenthost.UnDock(dockView);

                    //if (dockGroupControl.Items.Contains(dockView))
                    //{
                    //    dockGroupControl.Items.Remove(dockView);
                    //}
                    //else
                    //{
                    //    dockGroupControl.Items.Remove(dockView.Content);
                    //}

                    dragGrip.CancelDrag();

                    dragGrip.IsWindowTitleBar = true;

                    FloatingWindow floatwindow = new FloatingWindow()
                    {
                        Top = undockedPosition.Y,
                        Left = undockedPosition.X,
                        Height = undockedPosition.Height,
                        Width = undockedPosition.Width
                    };
                    //DockGroupControl group = new DockGroupControl();
                    //group.Items.Add(dockView);
                    floatwindow.Content = dockView;
                    floatwindow.Show();

                    DockManager.CurrentDraggedContext.DraggedWindow = floatwindow;

                    floatwindow.DragMove();
                }
            }
            else
            {
                Window draggedwindow = Window.GetWindow(dragGrip);
                DockManager.CurrentDraggedContext.DraggedWindow = draggedwindow;

                if (draggedwindow != null)
                {
                    DockManager.UpdateAdorners(dragGrip, e, draggedwindow);
                    DockManager.UpdatePreviewWindow(dragGrip, e, draggedwindow);
                    DockManager.UpdateIsFloatingWindowDragWithin(e, draggedwindow);
                }
            }
        }


        private static void OnDockViewDragGripDragCompletedAbsolute(object sender, DragAbsoluteCompletedEventArgs e)
        {
            using (var currentcontext = DockManager.CurrentDraggedContext)
            {
                if (currentcontext.HitDockSiteAdorner == null)
                {
                    return;
                }

                var currentitem = currentcontext.DockDragGrip.FindAncestor<DockItem>();
                var olditem = currentcontext.HitDockTarget.FindAncestor<DockItem>();
                var host = (IDockHost)currentcontext.HitDockTarget.FindAncestor<DependencyObject>(_p => _p is IDockHost);
                var direction = currentcontext.HitDockSiteAdorner.DockDirection;


                host.Dock(direction, olditem, currentitem);
                //TODO 停靠完成处理
            }
        }







        public static void UpdateAdorners(DockDragGrip draggrip, DragAbsoluteEventArgs e, Window draggedwindow)
        {
            //非当前拖动窗体和非DockAdornerWindow窗体并且某个上级类型为DockTarget
            DockSiteHitTestResult hitresult = DockManager.FindValidHitElement<DockTarget>(
                e.ScreenPoint,
                _ds => _ds.Visual != draggedwindow
                        && !(_ds.Visual is DockAdornerWindow),
                out DockTarget dockTarget
            );

            DockManager.CurrentDraggedContext.HitDockTarget = dockTarget;
            DockManager.CurrentDraggedContext.DragOverWindow = hitresult == null ? null : Window.GetWindow(hitresult.VisualHit);

            if (dockTarget != null)
            {
                List<DockTarget> ancestortargets = dockTarget?.FindAncestorAll<DockTarget>(_p => _p.FindMode == FindMode.Always).ToList();

                ancestortargets.Insert(0, dockTarget);

                DockManager.CurrentDraggedContext.CleanupAdorners(ancestortargets);

                foreach (var target in ancestortargets)
                {
                    if (!DockManager.CurrentDraggedContext.Adorners.ContainsKey(target))
                    {
                        List<DockAdornerWindow> group = new List<DockAdornerWindow>();

                        bool typebouth = false;

                        switch (target.DockTargetType)
                        {
                            case DockTargetType.Outside:
                                {
                                    bool oriboth = false;
                                    switch ((Orientation?)target.GetValue(DockTarget.OrientationProperty))
                                    {
                                        case Orientation.Horizontal:
                                            {
                                                group.Add(
                                                    new DockAdornerWindow(hitresult.DockSite.Handle)
                                                    {
                                                        AdornedElement = target,
                                                        DockDirection = DockDirection.Right
                                                    });

                                                group.Add(
                                                    new DockAdornerWindow(hitresult.DockSite.Handle)
                                                    {
                                                        AdornedElement = target,
                                                        DockDirection = DockDirection.Left
                                                    });

                                                if (oriboth)
                                                {
                                                    goto case Orientation.Vertical;
                                                }
                                            }
                                            break;

                                        case Orientation.Vertical:
                                            {
                                                group.Add(
                                                    new DockAdornerWindow(hitresult.DockSite.Handle)
                                                    {
                                                        AdornedElement = target,
                                                        DockDirection = DockDirection.Top
                                                    });

                                                group.Add(
                                                    new DockAdornerWindow(hitresult.DockSite.Handle)
                                                    {
                                                        AdornedElement = target,
                                                        DockDirection = DockDirection.Bottom
                                                    });
                                            }
                                            break;

                                        default:
                                            {
                                                oriboth = true;
                                                goto case Orientation.Horizontal;
                                            }
                                    }

                                    if (typebouth)
                                    {
                                        goto case DockTargetType.Center;
                                    }
                                }
                                break;

                            case DockTargetType.Center:
                                {
                                    group.Add(
                                        new DockAdornerWindow(hitresult.DockSite.Handle)
                                        {
                                            AdornedElement = target,
                                            DockDirection = DockDirection.Fill,
                                            Orientation = (Orientation?)target.GetValue(DockTarget.OrientationProperty),
                                            AreInnerTargetsEnabled = true,
                                            IsInnerCenterTargetEnabled = true,
                                        });
                                }
                                break;

                            case DockTargetType.Both:
                                {
                                    typebouth = true;
                                    goto case DockTargetType.Outside;
                                }
                        }

                        foreach (var item in group)
                        {
                            item.PrepareAndShow();
                        }

                        DockManager.CurrentDraggedContext.Adorners.Add(target, group);
                    }
                }
            }
            else
            {
                DockManager.CurrentDraggedContext.CleanupAdorners(null);
            }
        }

        public static void UpdatePreviewWindow(DockDragGrip draggrip, DragAbsoluteEventArgs e, Window draggedwindow)
        {
            DockSiteHitTestResult hitresult = DockManager.FindValidHitElement<DockSiteAdorner>(
                e.ScreenPoint,
                _ds => _ds.Visual != draggedwindow,
                out DockSiteAdorner docksiteadorner
            );

            DockAdornerWindow adornerwindow = docksiteadorner?.FindAncestorOrSelf<DockAdornerWindow>();

            if (docksiteadorner != null
                && adornerwindow != null
                && adornerwindow.AdornedElement != null)
            {
                SetupDockPreviewArgs previewargs = new SetupDockPreviewArgs
                {
                    previewRect = DockManager.GetDockPreviewRect(
                                                    docksiteadorner.DockDirection,
                                                    adornerwindow.AdornedElement,
                                                    draggrip.FindAncestor<DockItem>()
                                                ),
                    dockTargetType = DockTargetType.Outside,
                    screenPoint = e.ScreenPoint,
                    dockDirection = docksiteadorner.DockDirection,
                    adornedElement = adornerwindow.AdornedElement
                };

                DockManager.CurrentDraggedContext.DockPreviewWindow.SetupDockPreview(previewargs);
                DockManager.CurrentDraggedContext.DockPreviewWindow.Show(hitresult.DockSite.Handle);

                //Site窗口置于预览窗口之上
                NativeMethods.SetWindowPos(
                    DockManager.CurrentDraggedContext.DockPreviewWindow.Handle,
                    adornerwindow.Handle,
                    0,
                    0,
                    0,
                    0,
                    16403);
            }
            else
            {
                DockManager.CurrentDraggedContext.HideDockPreview();
            }
        }

        private static void UpdateIsFloatingWindowDragWithin(DragAbsoluteEventArgs e, Window draggedwindow)
        {
            DockSiteHitTestResult hitresult = DockManager.FindValidHitElement<DockSiteAdorner>(
                e.ScreenPoint,
                _ds => _ds.Visual != draggedwindow,
                out DockSiteAdorner hitdocksite
            );

            var oldadorner = DockManager.CurrentDraggedContext.HitDockSiteAdorner;
            if (oldadorner != null
                && oldadorner != hitdocksite)
            {
                oldadorner.IsHighlighted = false;
            }

            DockManager.CurrentDraggedContext.HitDockSiteAdorner = hitdocksite;
            if (hitresult != null)
            {
                hitdocksite.IsHighlighted = true;
            }
        }

        private static Rect GetDockPreviewRect(DockDirection dockDirection, FrameworkElement docktarget, DockItem drageddockItem)
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

                if (panel != null)
                {
                    if (orientation == panel.Orientation)
                    {
                        return PreviewDockSameOrientation(dockDirection, panel, docktarget, drageddockItem, orientation);
                    }
                }
                //非Fill情况下 分半屏
                return PreviewDockCounterOrientation(dockDirection, docktarget, orientation);
            }

            return PreviewDockFill(docktarget);
        }

        private static Rect PreviewDockSameOrientation(DockDirection dockDirection,
            SplitterPanel targetpanel,
            FrameworkElement docktarget,
            DockItem drageddockItem,
            Orientation orientation)
        {
            //找到父SplitterPanel 和 停靠目标 SplitterItem 的索引位置
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

        private static Rect PreviewDockCounterOrientation(DockDirection dockDirection,
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

        private static Rect PreviewDockFill(FrameworkElement adornedElement)
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








        public static void RegisterDockSite(Window window)
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

        public static void RegisterDockSite(Visual visual, IntPtr hWnd)
        {
            if (_visualToSite.ContainsKey(visual))
            {
                if (_visualToSite[visual].Handle != hWnd)
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
                _visualToSite[visual] = value;
                _hwndToSite[hWnd] = value;
            }
        }

        private static void OnSourceInitialized(object sender, EventArgs args)
        {
            Window window = (Window)sender;
            window.SourceInitialized -= OnSourceInitialized;
            RegisterDockSite(window);
        }

        public static void UnregisterDockSite(Visual visual)
        {
            if (_visualToSite.TryGetValue(visual, out DockSite value))
            {
                _visualToSite.Remove(visual);
                _hwndToSite.Remove(value.Handle);
            }
        }


        private static DockSiteHitTestResult FindValidHitElement<T>(Point point, Predicate<DockSite> predicate, out T target) where T : DependencyObject
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

        private static List<DockSiteHitTestResult> FindHitElements(Point point, Predicate<DockSite> predicate)
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

        private static List<DockSite> GetSortedDockSites()
        {
            void AddDockSiteFromHwnd(List<DockSite> sortedSites, IntPtr hWnd)
            {
                if (_hwndToSite.TryGetValue(hWnd, out DockSite value))
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
