using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Unicorn.ViewManager
{
    public class SplitterMeasureData
    {
        /// <summary>
        /// Gets the UIElement being measured.
        /// </summary>
        public UIElement Element
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the SplitterLength associated with the UIElement.
        /// </summary>
        public SplitterLength AttachedLength
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not the element reached its minimum
        /// size during Measure.
        /// </summary>
        public bool IsMinimumReached
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether or not the element reached its maximum
        /// size during Measure.
        /// </summary>
        public bool IsMaximumReached
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bounds the element was assigned, relative
        /// to the panel it is contained in.
        /// </summary>
        public Rect MeasuredBounds
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a new SplitterMeasureData from a UIElement.
        /// </summary>
        /// <param name="element">The element being measured.</param>
        public SplitterMeasureData(UIElement element)
        {
            Element = element;
            AttachedLength = SplitterPanel.GetSplitterLength(element);
        }

        /// <summary>
        /// Constructs a new sequence of SplitterMeasureData instances from a
        /// sequence of UIElements.
        /// </summary>
        /// <param name="elements">The sequence of UIElements.</param>
        /// <returns>A sequence of SplitterMeasureData instances based
        /// on the sequence of UIElements.</returns>
        public static IList<SplitterMeasureData> FromElements(IList elements)
        {
            List<SplitterMeasureData> list = new List<SplitterMeasureData>(elements.Count);
            foreach (UIElement element in elements)
            {
                if (element != null)
                {
                    list.Add(new SplitterMeasureData(element));
                }
            }
            return list;
        }
    }

}
