using System;
using System.Reflection;
using System.Windows;

namespace Unicorn.ViewManager.Internal
{
    internal static class CommonUtilities
	{
		public static Rect Resize(
		  this Rect rect,
		  Vector positionChangeDelta,
		  Vector sizeChangeDelta,
		  Size minSize,
		  Size maxSize)
		{
			double width = Math.Min(Math.Max(minSize.Width, rect.Width + sizeChangeDelta.X), maxSize.Width);
			double height = Math.Min(Math.Max(minSize.Height, rect.Height + sizeChangeDelta.Y), maxSize.Height);
			double right = rect.Right;
			double bottom = rect.Bottom;
			return new Rect(Math.Min(right - minSize.Width, Math.Max(right - maxSize.Width, rect.Left + positionChangeDelta.X)), Math.Min(bottom - minSize.Height, Math.Max(bottom - maxSize.Height, rect.Top + positionChangeDelta.Y)), width, height);
		}

		public static Uri MakePackUri(Assembly assembly, string path) => new Uri(string.Format("pack://application:,,,/{0};component/{1}", (object)assembly.GetName().Name, (object)path), UriKind.Absolute);
	}
}
