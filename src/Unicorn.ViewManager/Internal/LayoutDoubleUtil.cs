using System.Windows;

namespace Unicorn.ViewManager.Internal
{
    internal static class LayoutDoubleUtil
    {
        private const double eps = 1.53E-06;

        /// <summary>
        /// Determines if two double values are close to each other.
        /// </summary>
        /// <param name="value1">First value to compare</param>
        /// <param name="value2">Second value to compare</param>
        /// <returns>True if the values are close, false if they are not</returns>
        public static bool AreClose(this double value1, double value2)
        {
            if (IsNonreal(value1) || IsNonreal(value2))
            {
                return value1.CompareTo(value2) == 0;
            }
            if (value1 == value2)
            {
                return true;
            }
            double num = value1 - value2;
            if (num < 1.53E-06)
            {
                return num > -1.53E-06;
            }
            return false;
        }

        /// <summary>
        /// Determines if two rectangles are close to each other.  The rectangles are close
        /// if their origins are close and their sizes are close.
        /// </summary>
        /// <param name="rect1">First value to compare</param>
        /// <param name="rect2">Second value to compare</param>
        /// <returns>True if the values are close, false if they are not</returns>
        public static bool AreClose(Rect rect1, Rect rect2)
        {
            if (!AreClose(rect1.Location, rect2.Location))
            {
                return false;
            }
            if (!AreClose(rect1.Size, rect2.Size))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if two sizes are close to each other.  The sizes are close
        /// if their widths are close and their heights are close.
        /// </summary>
        /// <param name="size1">First value to compare</param>
        /// <param name="size2">Second value to compare</param>
        /// <returns>True if the values are close, false if they are not</returns>
        public static bool AreClose(this Size size1, Size size2)
        {
            if (!AreClose(size1.Width, size2.Width))
            {
                return false;
            }
            if (!AreClose(size1.Height, size2.Height))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if two points are close to each other.  The points are close
        /// if their x-coordinates are close and their y-coordinates are close.
        /// </summary>
        /// <param name="size1">First value to compare</param>
        /// <param name="size2">Second value to compare</param>
        /// <returns>True if the values are close, false if they are not</returns>
        public static bool AreClose(this Point size1, Point size2)
        {
            if (!AreClose(size1.X, size2.X))
            {
                return false;
            }
            if (!AreClose(size1.Y, size2.Y))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if one double value is less than another, but not close to it.
        /// </summary>
        /// <param name="value1">First value to compare</param>
        /// <param name="value2">Second value to compare</param>
        /// <returns>True if <paramref name="value1" /> is less than <paramref name="value2" />, otherwise false</returns>
        public static bool LessThan(this double value1, double value2)
        {
            if (value1 < value2)
            {
                return !AreClose(value1, value2);
            }
            return false;
        }

        /// <summary>
        /// Determines if one double value is less than or close to another.
        /// </summary>
        /// <param name="value1">First value to compare</param>
        /// <param name="value2">Second value to compare</param>
        /// <returns>True if <paramref name="value1" /> is less than or close to <paramref name="value2" />, otherwise false</returns>
        public static bool LessThanOrClose(this double value1, double value2)
        {
            if (!(value1 < value2))
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        /// <summary>
        /// Determines if one double value is greater than another, but not close to it.
        /// </summary>
        /// <param name="value1">First value to compare</param>
        /// <param name="value2">Second value to compare</param>
        /// <returns>True if <paramref name="value1" /> is greater than <paramref name="value2" />, otherwise false</returns>
        public static bool GreaterThan(this double value1, double value2)
        {
            if (value1 > value2)
            {
                return !AreClose(value1, value2);
            }
            return false;
        }

        /// <summary>
        /// Determines if one double value is greater than or close to another.
        /// </summary>
        /// <param name="value1">First value to compare</param>
        /// <param name="value2">Second value to compare</param>
        /// <returns>True if <paramref name="value1" /> is greater than or close to <paramref name="value2" />, otherwise false</returns>
        public static bool GreaterThanOrClose(this double value1, double value2)
        {
            if (!(value1 > value2))
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        /// <summary>
        /// Determines whether a double value represents a non-real value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if the value is non-real, false otherwise.</returns>
        public static bool IsNonreal(this double value)
        {
            if (!double.IsNaN(value))
            {
                return double.IsInfinity(value);
            }
            return true;
        }
    }
}
