using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Unicorn.ViewManager
{
    public class SplitterPanel : Panel
    {
        public static readonly DependencyProperty SplitterLengthProperty;

        public static readonly DependencyProperty OrientationProperty;

        public static readonly DependencyProperty ShowResizePreviewProperty;

        public static readonly DependencyProperty MinimumLengthProperty;

        public static readonly DependencyProperty MaximumLengthProperty;

        private static readonly DependencyPropertyKey ActualSplitterLengthPropertyKey;

        private static readonly DependencyPropertyKey IndexPropertyKey;

        private static readonly DependencyPropertyKey IsFirstPropertyKey;

        private static readonly DependencyPropertyKey IsLastPropertyKey;

        public static readonly DependencyProperty ActualSplitterLengthProperty;

        public static readonly DependencyProperty IndexProperty;

        public static readonly DependencyProperty IsFirstProperty;

        public static readonly DependencyProperty IsLastProperty;

        private SplitterResizePreviewWindow currentPreviewWindow;

        private bool IsShowingResizePreview => currentPreviewWindow != null;

        public Orientation Orientation
        {
            get
            {
                return (Orientation)GetValue(OrientationProperty);
            }
            set
            {
                SetValue(OrientationProperty, value);
            }
        }

        public bool ShowResizePreview
        {
            get
            {
                return (bool)GetValue(ShowResizePreviewProperty);
            }
            set
            {
                SetValue(ShowResizePreviewProperty, value);
            }
        }

        static SplitterPanel()
        {
            SplitterLengthProperty = DependencyProperty.RegisterAttached("SplitterLength", typeof(SplitterLength), typeof(SplitterPanel), new FrameworkPropertyMetadata(new SplitterLength(100.0), FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange));
            OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitterPanel), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.AffectsMeasure));
            ShowResizePreviewProperty = DependencyProperty.Register("ShowResizePreview", typeof(bool), typeof(SplitterPanel), new FrameworkPropertyMetadata(false));
            MinimumLengthProperty = DependencyProperty.RegisterAttached("MinimumLength", typeof(double), typeof(SplitterPanel), new FrameworkPropertyMetadata(0.0d));
            MaximumLengthProperty = DependencyProperty.RegisterAttached("MaximumLength", typeof(double), typeof(SplitterPanel), new FrameworkPropertyMetadata(1.7976931348623157E+308));
            ActualSplitterLengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ActualSplitterLength", typeof(double), typeof(SplitterPanel), new FrameworkPropertyMetadata(0.0d));
            IndexPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Index", typeof(int), typeof(SplitterPanel), new FrameworkPropertyMetadata(-1));
            IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof(bool), typeof(SplitterPanel), new FrameworkPropertyMetadata(false));
            IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof(bool), typeof(SplitterPanel), new FrameworkPropertyMetadata(false));
            ActualSplitterLengthProperty = ActualSplitterLengthPropertyKey.DependencyProperty;
            IndexProperty = IndexPropertyKey.DependencyProperty;
            IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
            IsLastProperty = IsLastPropertyKey.DependencyProperty;
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterPanel), new FrameworkPropertyMetadata(typeof(SplitterPanel)));
        }

        public SplitterPanel()
        {
            AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnSplitterDragStarted));
        }

        public static double GetActualSplitterLength(UIElement element)
        {
            return (double)element.GetValue(ActualSplitterLengthProperty);
        }

        protected static void SetActualSplitterLength(UIElement element, double value)
        {
            element.SetValue(ActualSplitterLengthPropertyKey, value);
        }

        public static int GetIndex(UIElement element)
        {
            return (int)element.GetValue(IndexProperty);
        }

        public static bool GetIsFirst(UIElement element)
        {
            return (bool)element.GetValue(IsFirstProperty);
        }

        protected static void SetIsFirst(UIElement element, bool value)
        {
            element.SetValue(IsFirstPropertyKey, value);
        }

        public static bool GetIsLast(UIElement element)
        {
            return (bool)element.GetValue(IsLastProperty);
        }

        protected static void SetIsLast(UIElement element, bool value)
        {
            element.SetValue(IsLastPropertyKey, value);
        }

        protected static void SetIndex(UIElement element, int value)
        {
            element.SetValue(IndexPropertyKey, value);
        }

        public static SplitterLength GetSplitterLength(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (SplitterLength)element.GetValue(SplitterLengthProperty);
        }

        public static void SetSplitterLength(UIElement element, SplitterLength value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(SplitterLengthProperty, value);
        }

        public static double GetMinimumLength(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (double)element.GetValue(MinimumLengthProperty);
        }

        public static void SetMinimumLength(UIElement element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(MinimumLengthProperty, value);
        }

        public static double GetMaximumLength(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (double)element.GetValue(MaximumLengthProperty);
        }

        public static void SetMaximumLength(UIElement element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(MaximumLengthProperty, value);
        }

        /// <summary>
        /// This method updates the IndexProperty, IsFirstProperty, and IsLastProperty
        /// on all of the child UIElements in this collection.  These properties
        /// are used by the SplitterItem style to determine if the splitter should be shown
        /// or not.
        /// </summary>
        private void UpdateIndices()
        {
            int count = base.InternalChildren.Count;
            int num = base.InternalChildren.Count - 1;
            for (int i = 0; i < count; i++)
            {
                UIElement uIElement = base.InternalChildren[i];
                if (uIElement != null)
                {
                    SetIndex(uIElement, i);
                    SetIsFirst(uIElement, i == 0);
                    SetIsLast(uIElement, i == num);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            UpdateIndices();
            return Measure(availableSize, Orientation, SplitterMeasureData.FromElements(base.InternalChildren), remeasureElements: true);
        }

        /// <summary>
        /// When there is an infinite size available in the stacking direction, there are no need for constraints
        /// and we can just measure the elements with the given size.
        /// </summary>
        /// <param name="availableSize">The size available to the control.</param>
        /// <param name="orientation">The control's orientation.</param>
        /// <param name="uiElements">The enumeration of child UIElements to measure and arrange.</param>
        /// <param name="remeasureElements">True to actually remeasure the child elements and attach
        /// the new layout information to them.  To perform a non-invasive preview of the layout, pass false.</param>
        /// <returns>The calculated size required for this control.</returns>
        private static Size MeasureNonreal(Size availableSize, Orientation orientation, IEnumerable<SplitterMeasureData> measureData, bool remeasureElements)
        {
            double num = 0.0;
            double num2 = 0.0;
            foreach (SplitterMeasureData measureDatum in measureData)
            {
                if (remeasureElements)
                {
                    measureDatum.Element.Measure(availableSize);
                }
                if (orientation == Orientation.Horizontal)
                {
                    num += measureDatum.Element.DesiredSize.Width;
                    num2 = Math.Max(num2, measureDatum.Element.DesiredSize.Height);
                }
                else
                {
                    num = Math.Max(num, measureDatum.Element.DesiredSize.Width);
                    num2 += measureDatum.Element.DesiredSize.Height;
                }
            }
            Rect measuredBounds = new Rect(0.0, 0.0, num, num2);
            foreach (SplitterMeasureData measureDatum2 in measureData)
            {
                if (orientation == Orientation.Horizontal)
                {
                    measuredBounds.Width = measureDatum2.Element.DesiredSize.Width;
                    measureDatum2.MeasuredBounds = measuredBounds;
                    measuredBounds.X += measuredBounds.Width;
                }
                else
                {
                    measuredBounds.Height = measureDatum2.Element.DesiredSize.Height;
                    measureDatum2.MeasuredBounds = measuredBounds;
                    measuredBounds.Y += measuredBounds.Height;
                }
            }
            return new Size(num, num2);
        }

        static bool IsNonreal( double value)
        {
            if (!double.IsNaN(value))
            {
                return double.IsInfinity(value);
            }
            return true;
        }

        /// <summary>
        /// This method is used to measure and arrange a set of child elements.  This method is used
        /// both for the normal Measure/Arrange pass, and also to determine where an item would
        /// show up if it were added to this panel.  This is used for docking previews.
        /// </summary>
        /// <param name="availableSize">The size available to the control.</param>
        /// <param name="orientation">The control's orientation.</param>
        /// <param name="measureData">The enumeration of child UIElements to measure and arrange.</param>
        /// <param name="remeasureElements">True to actually remeasure the child elements and attach
        /// the new layout information to them.  To perform a non-invasive preview of the layout, pass false.</param>
        /// <returns>The calculated size required for this control.</returns>
        public static Size Measure(Size availableSize, Orientation orientation, IEnumerable<SplitterMeasureData> measureData, bool remeasureElements)
        {
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            double num7 = 0.0;
            double num8 = 0.0;
            double num9 = 1.7976931348623157E+308;
            if ((orientation == Orientation.Horizontal && IsNonreal(availableSize.Width)) || (orientation == Orientation.Vertical && IsNonreal(availableSize.Height)))
            {
                return MeasureNonreal(availableSize, orientation, measureData, remeasureElements);
            }
            foreach (SplitterMeasureData measureDatum in measureData)
            {
                SplitterLength attachedLength = measureDatum.AttachedLength;
                num8 = GetMinimumLength(measureDatum.Element);
                if (attachedLength.IsStretch)
                {
                    num += attachedLength.Value;
                    num5 += num8;
                }
                else
                {
                    num2 += attachedLength.Value;
                    num4 += num8;
                }
                measureDatum.IsMinimumReached = false;
                measureDatum.IsMaximumReached = false;
            }
            num3 = num5 + num4;
            num6 = availableSize.Width;
            num7 = availableSize.Height;
            double num10 = (orientation == Orientation.Horizontal) ? num6 : num7;
            double num11 = (num2 == 0.0) ? 0.0 : Math.Max(0.0, num10 - num);
            double num12 = (num11 == 0.0) ? num10 : num;
            if (num3 <= num10)
            {
                bool flag = false;
                foreach (SplitterMeasureData measureDatum2 in measureData)
                {
                    SplitterLength attachedLength2 = measureDatum2.AttachedLength;
                    num9 = GetMaximumLength(measureDatum2.Element);
                    if (attachedLength2.IsStretch)
                    {
                        double num13 = (num == 0.0) ? 0.0 : (attachedLength2.Value / num * num12);
                        if (num13 > num9)
                        {
                            measureDatum2.IsMaximumReached = true;
                            flag = true;
                            if (num == attachedLength2.Value)
                            {
                                num = num9;
                                measureDatum2.AttachedLength = new SplitterLength(num9);
                            }
                            else
                            {
                                num -= attachedLength2.Value;
                                measureDatum2.AttachedLength = new SplitterLength(num);
                                num += num;
                            }
                            num11 = ((num2 == 0.0) ? 0.0 : Math.Max(0.0, num10 - num));
                            num12 = ((num11 == 0.0) ? num10 : num);
                        }
                    }
                }
                if (num11 < num4)
                {
                    num11 = num4;
                    num12 = num10 - num11;
                }
                foreach (SplitterMeasureData measureDatum3 in measureData)
                {
                    SplitterLength attachedLength3 = measureDatum3.AttachedLength;
                    num8 = GetMinimumLength(measureDatum3.Element);
                    if (attachedLength3.IsFill)
                    {
                        double num13 = (num2 == 0.0) ? 0.0 : (attachedLength3.Value / num2 * num11);
                        if (num13 < num8)
                        {
                            measureDatum3.IsMinimumReached = true;
                            num11 -= num8;
                            num2 -= attachedLength3.Value;
                        }
                    }
                    else
                    {
                        double num13 = (num == 0.0) ? 0.0 : (attachedLength3.Value / num * num12);
                        if (num13 < num8)
                        {
                            measureDatum3.IsMinimumReached = true;
                            num12 -= num8;
                            num -= attachedLength3.Value;
                        }
                    }
                }
            }
            Size availableSize2 = new Size(num6, num7);
            Rect rect = new Rect(0.0, 0.0, num6, num7);
            foreach (SplitterMeasureData measureDatum4 in measureData)
            {
                SplitterLength attachedLength4 = measureDatum4.AttachedLength;
                double num14 = measureDatum4.IsMinimumReached ? GetMinimumLength(measureDatum4.Element) : ((!attachedLength4.IsFill) ? ((num == 0.0) ? 0.0 : (attachedLength4.Value / num * num12)) : ((num2 == 0.0) ? 0.0 : (attachedLength4.Value / num2 * num11)));
                if (remeasureElements)
                {
                    SetActualSplitterLength(measureDatum4.Element, num14);
                }
                if (orientation == Orientation.Horizontal)
                {
                    availableSize2.Width = num14;
                    measureDatum4.MeasuredBounds = new Rect(rect.Left, rect.Top, num14, rect.Height);
                    rect.X += num14;
                    if (remeasureElements)
                    {
                        measureDatum4.Element.Measure(availableSize2);
                    }
                }
                else
                {
                    availableSize2.Height = num14;
                    measureDatum4.MeasuredBounds = new Rect(rect.Left, rect.Top, rect.Width, num14);
                    rect.Y += num14;
                    if (remeasureElements)
                    {
                        measureDatum4.Element.Measure(availableSize2);
                    }
                }
            }
            return new Size(num6, num7);
        }

        /// <summary>
        /// Arranges the elements based on attached arrangement information
        /// calculated in Measure.
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect finalRect = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            foreach (UIElement internalChild in base.InternalChildren)
            {
                if (internalChild != null)
                {
                    double actualSplitterLength = GetActualSplitterLength(internalChild);
                    if (Orientation == Orientation.Horizontal)
                    {
                        finalRect.Width = actualSplitterLength;
                        internalChild.Arrange(finalRect);
                        finalRect.X += actualSplitterLength;
                    }
                    else
                    {
                        finalRect.Height = actualSplitterLength;
                        internalChild.Arrange(finalRect);
                        finalRect.Y += actualSplitterLength;
                    }
                }
            }
            return finalSize;
        }

        /// <summary>
        /// Handles the resize of splitters for this panel.  This could
        /// show the preview resize window for this panel, if ShowResizePreview
        /// is true.
        /// </summary>
        /// <param name="sender">The sender, which may or may not be a SplitterGrip.</param>
        /// <param name="args">The event args.</param>
        private void OnSplitterDragStarted(object sender, DragStartedEventArgs args)
        {
            SplitterGrip splitterGrip = args.OriginalSource as SplitterGrip;
            if (splitterGrip != null)
            {
                args.Handled = true;
                splitterGrip.DragDelta += OnSplitterResized;
                splitterGrip.DragCompleted += OnSplitterDragCompleted;
                if (ShowResizePreview)
                {
                    currentPreviewWindow = new SplitterResizePreviewWindow();
                    currentPreviewWindow.Show(splitterGrip);
                }
            }
        }

        /// <summary>
        /// Commits the resize for a splitter if a drag operation was successful.
        /// </summary>
        /// <param name="sender">The sender, which should be a SplitterGrip.</param>
        /// <param name="args">The event args.</param>
        private void OnSplitterDragCompleted(object sender, DragCompletedEventArgs args)
        {
            SplitterGrip splitterGrip = sender as SplitterGrip;
            if (splitterGrip != null)
            {
                args.Handled = true;
                if (IsShowingResizePreview)
                {
                    currentPreviewWindow.Hide();
                    currentPreviewWindow = null;
                    Point point = new Point(args.HorizontalChange, args.VerticalChange);
                    CommitResize(splitterGrip, point.X, point.Y);
                }
                splitterGrip.DragDelta -= OnSplitterResized;
                splitterGrip.DragCompleted -= OnSplitterDragCompleted;
            }
        }
        private void OnSplitterResized(object sender, DragDeltaEventArgs args)
        {
            SplitterGrip splitterGrip = sender as SplitterGrip;
            if (splitterGrip != null)
            {
                args.Handled = true;
                if (IsShowingResizePreview)
                {
                    TrackResizePreview(splitterGrip, args.HorizontalChange, args.VerticalChange);
                }
                else
                {
                    CommitResize(splitterGrip, args.HorizontalChange, args.VerticalChange);
                }
            }
        }
        private void CommitResize(SplitterGrip grip, double horizontalChange, double verticalChange)
        {
            if (GetResizeIndices(grip, out int _, out int resizeIndex, out int resizeIndex2))
            {
                double pixelAmount = (Orientation == Orientation.Horizontal) ? horizontalChange : verticalChange;
                ResizeChildren(resizeIndex, resizeIndex2, pixelAmount);
            }
        }
        private void TrackResizePreview(SplitterGrip grip, double horizontalChange, double verticalChange)
        {
            if (GetResizeIndices(grip, out int gripIndex, out int resizeIndex, out int resizeIndex2))
            {
                double pixelAmount = (Orientation == Orientation.Horizontal) ? horizontalChange : verticalChange;
                IList<SplitterMeasureData> list = SplitterMeasureData.FromElements(base.InternalChildren);
                ResizeChildrenCore(list[resizeIndex], list[resizeIndex2], pixelAmount);
                Measure(base.RenderSize, Orientation, list, remeasureElements: false);
                Point point = grip.TransformToAncestor(this).Transform(new Point(0.0, 0.0));
                if (Orientation == Orientation.Horizontal)
                {
                    point.X += list[gripIndex].MeasuredBounds.Width - base.InternalChildren[gripIndex].RenderSize.Width;
                }
                else
                {
                    point.Y += list[gripIndex].MeasuredBounds.Height - base.InternalChildren[gripIndex].RenderSize.Height;
                }
                point = PointToScreen(point);
                currentPreviewWindow.Move((double)(int)point.X, (double)(int)point.Y);
            }
        }
        private bool GetResizeIndices(SplitterGrip grip, out int gripIndex, out int resizeIndex1, out int resizeIndex2)
        {
            for (int i = 0; i < base.InternalChildren.Count; i++)
            {
                if (base.InternalChildren[i].IsAncestorOf(grip))
                {
                    gripIndex = i;
                    switch (grip.ResizeBehavior)
                    {
                        case GridResizeBehavior.PreviousAndNext:
                            resizeIndex1 = i - 1;
                            resizeIndex2 = i + 1;
                            break;
                        case GridResizeBehavior.CurrentAndNext:
                            resizeIndex1 = i;
                            resizeIndex2 = i + 1;
                            break;
                        case GridResizeBehavior.PreviousAndCurrent:
                            resizeIndex1 = i - 1;
                            resizeIndex2 = i;
                            break;
                        default:
                            throw new InvalidOperationException("BasedOnAlignment is not a valid resize behavior");
                    }
                    if (resizeIndex1 >= 0 && resizeIndex2 >= 0 && resizeIndex1 < base.InternalChildren.Count)
                    {
                        return resizeIndex2 < base.InternalChildren.Count;
                    }
                    return false;
                }
            }
            gripIndex = -1;
            resizeIndex1 = -1;
            resizeIndex2 = -1;
            return false;
        }

        internal void ResizeChildren(int index1, int index2, double pixelAmount)
        {
            SplitterMeasureData splitterMeasureData = new SplitterMeasureData(base.InternalChildren[index1]);
            SplitterMeasureData splitterMeasureData2 = new SplitterMeasureData(base.InternalChildren[index2]);
            if (ResizeChildrenCore(splitterMeasureData, splitterMeasureData2, pixelAmount))
            {
                SetSplitterLength(splitterMeasureData.Element, splitterMeasureData.AttachedLength);
                SetSplitterLength(splitterMeasureData2.Element, splitterMeasureData2.AttachedLength);
                InvalidateMeasure();
            }
        }

        private bool ResizeChildrenCore(SplitterMeasureData child1, SplitterMeasureData child2, double pixelAmount)
        {
            UIElement element = child1.Element;
            UIElement element2 = child2.Element;
            SplitterLength attachedLength = child1.AttachedLength;
            SplitterLength attachedLength2 = child2.AttachedLength;
            double actualSplitterLength = GetActualSplitterLength(element);
            double actualSplitterLength2 = GetActualSplitterLength(element2);
            double num = Math.Max(0.0, Math.Min(actualSplitterLength + actualSplitterLength2, actualSplitterLength + pixelAmount));
            double num2 = Math.Max(0.0, Math.Min(actualSplitterLength + actualSplitterLength2, actualSplitterLength2 - pixelAmount));
            double minimumLength = GetMinimumLength(element);
            double minimumLength2 = GetMinimumLength(element2);
            if (minimumLength + minimumLength2 <= num + num2)
            {
                if (num < minimumLength)
                {
                    num2 -= minimumLength - num;
                    num = minimumLength;
                }
                if (num2 < minimumLength2)
                {
                    num -= minimumLength2 - num2;
                    num2 = minimumLength2;
                }
                if ((attachedLength.IsFill && attachedLength2.IsFill) || (attachedLength.IsStretch && attachedLength2.IsStretch))
                {
                    child1.AttachedLength = new SplitterLength(num / (num + num2) * (attachedLength.Value + attachedLength2.Value), attachedLength.SplitterUnitType);
                    child2.AttachedLength = new SplitterLength(num2 / (num + num2) * (attachedLength.Value + attachedLength2.Value), attachedLength.SplitterUnitType);
                }
                else if (attachedLength.IsFill)
                {
                    child2.AttachedLength = new SplitterLength(num2, SplitterUnitType.Stretch);
                }
                else
                {
                    child1.AttachedLength = new SplitterLength(num, SplitterUnitType.Stretch);
                }
                return true;
            }
            return false;
        }
    }

}
