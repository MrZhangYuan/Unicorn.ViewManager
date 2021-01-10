using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Unicorn.ViewManager
{
    public class SplitterGrip : Thumb
    {
        public static readonly DependencyProperty OrientationProperty;

        public static readonly DependencyProperty ResizeBehaviorProperty;

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

        public GridResizeBehavior ResizeBehavior
        {
            get
            {
                return (GridResizeBehavior)GetValue(ResizeBehaviorProperty);
            }
            set
            {
                SetValue(ResizeBehaviorProperty, value);
            }
        }

        static SplitterGrip()
        {
            OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitterGrip), new FrameworkPropertyMetadata(Orientation.Vertical));
            ResizeBehaviorProperty = DependencyProperty.Register("ResizeBehavior", typeof(GridResizeBehavior), typeof(SplitterGrip), new FrameworkPropertyMetadata(GridResizeBehavior.CurrentAndNext));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterGrip), new FrameworkPropertyMetadata(typeof(SplitterGrip)));
        }
    }

}
