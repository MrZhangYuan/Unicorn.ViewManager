using System;
using System.Windows;
using System.Windows.Controls;

namespace Unicorn.ViewManager
{
    public class SplitterItemsControl : ItemsControl
    {
        public static readonly DependencyProperty OrientationProperty;

        public static readonly DependencyProperty SplitterGripSizeProperty;

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

        static SplitterItemsControl()
        {
            OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitterItemsControl), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure, OnOrientationChanged));
            SplitterGripSizeProperty = DependencyProperty.RegisterAttached("SplitterGripSize", typeof(double), typeof(SplitterItemsControl), new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.Inherits));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterItemsControl), new FrameworkPropertyMetadata(typeof(SplitterItemsControl)));
        }

        public static double GetSplitterGripSize(DependencyObject element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            return (double)element.GetValue(SplitterGripSizeProperty);
        }

        public static void SetSplitterGripSize(DependencyObject element, double value)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.SetValue(SplitterGripSizeProperty, value);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SplitterItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SplitterItem();
        }

        private static void OnOrientationChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            SplitterItemsControl splitterItemsControl = (SplitterItemsControl)sender;
            splitterItemsControl.OnOrientationChanged();
        }

        protected virtual void OnOrientationChanged()
        {
        }
    }

}
