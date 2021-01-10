using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Unicorn.ViewManager.Internal
{


    internal static class DpiHelper
	{
		public class InnerDpiHelper
		{
			private delegate void PixelProcessor(ref byte alpha, ref byte red, ref byte green, ref byte blue);

			protected const double DefaultLogicalDpi = 96.0;

			private MatrixTransform transformFromDevice;

			private MatrixTransform transformToDevice;
			public MatrixTransform TransformFromDevice => transformFromDevice;

			public MatrixTransform TransformToDevice => transformToDevice;

			public double DeviceDpiX
			{
				get;
				private set;
			}

			public double DeviceDpiY
			{
				get;
				private set;
			}

			public double LogicalDpiX
			{
				get;
				private set;
			}

			public double LogicalDpiY
			{
				get;
				private set;
			}

			public double DeviceToLogicalUnitsScalingFactorX => TransformFromDevice.Matrix.M11;

			public double DeviceToLogicalUnitsScalingFactorY => TransformFromDevice.Matrix.M22;

			public double LogicalToDeviceUnitsScalingFactorX => TransformToDevice.Matrix.M11;

			public double LogicalToDeviceUnitsScalingFactorY => TransformToDevice.Matrix.M22;

			public static InnerDpiHelper GetHelper(int zoomPercent)
			{
				return new InnerDpiHelper(96.0 * (double)zoomPercent / 100.0);
			}

			protected InnerDpiHelper(double logicalDpi)
			{
				LogicalDpiX = logicalDpi;
				LogicalDpiY = logicalDpi;
				IntPtr dC = NativeMethods.GetDC(IntPtr.Zero);
				if (dC != IntPtr.Zero)
				{
					DeviceDpiX = (double)NativeMethods.GetDeviceCaps(dC, 88);
					DeviceDpiY = (double)NativeMethods.GetDeviceCaps(dC, 90);
					NativeMethods.ReleaseDC(IntPtr.Zero, dC);
				}
				else
				{
					DeviceDpiX = LogicalDpiX;
					DeviceDpiY = LogicalDpiY;
				}
				System.Windows.Media.Matrix identity = System.Windows.Media.Matrix.Identity;
				System.Windows.Media.Matrix identity2 = System.Windows.Media.Matrix.Identity;
				identity.Scale(DeviceDpiX / LogicalDpiX, DeviceDpiY / LogicalDpiY);
				identity2.Scale(LogicalDpiX / DeviceDpiX, LogicalDpiY / DeviceDpiY);
				transformFromDevice = new MatrixTransform(identity2);
				transformFromDevice.Freeze();
				transformToDevice = new MatrixTransform(identity);
				transformToDevice.Freeze();
			}
			public System.Windows.Point LogicalToDeviceUnits(System.Windows.Point logicalPoint)
			{
				return TransformToDevice.Transform(logicalPoint);
			}

			public Rect LogicalToDeviceUnits(Rect logicalRect)
			{
				Rect result = logicalRect;
				result.Transform(TransformToDevice.Matrix);
				return result;
			}

			public System.Windows.Size LogicalToDeviceUnits(System.Windows.Size logicalSize)
			{
				return new System.Windows.Size(logicalSize.Width * LogicalToDeviceUnitsScalingFactorX, logicalSize.Height * LogicalToDeviceUnitsScalingFactorY);
			}

			public System.Windows.Point DeviceToLogicalUnits(System.Windows.Point devicePoint)
			{
				return TransformFromDevice.Transform(devicePoint);
			}

			public Rect DeviceToLogicalUnits(Rect deviceRect)
			{
				Rect result = deviceRect;
				result.Transform(TransformFromDevice.Matrix);
				return result;
			}

			public System.Windows.Size DeviceToLogicalUnits(System.Windows.Size deviceSize)
			{
				return new System.Windows.Size(deviceSize.Width * DeviceToLogicalUnitsScalingFactorX, deviceSize.Height * DeviceToLogicalUnitsScalingFactorY);
			}

			public Rect GetDeviceRect(Window window)
			{
				NativeMethods.GetWindowRect(new WindowInteropHelper(window).Handle, out RECT lpRect);
				return new Rect(new System.Windows.Point((double)lpRect.Left, (double)lpRect.Top), new System.Windows.Size((double)lpRect.Width, (double)lpRect.Height));
			}
		}

		public class DpiHelperImplementation : InnerDpiHelper
		{
			public DpiHelperImplementation()
				: base(96.0)
			{
			}
		}

		public static DpiHelperImplementation Instance
		{
			get;
			private set;
		}

		public static double DeviceToLogicalUnitsScalingFactorX => Instance.DeviceToLogicalUnitsScalingFactorX;

		public static double DeviceToLogicalUnitsScalingFactorY => Instance.DeviceToLogicalUnitsScalingFactorY;

		public static double LogicalToDeviceUnitsScalingFactorX => Instance.LogicalToDeviceUnitsScalingFactorX;

		public static double LogicalToDeviceUnitsScalingFactorY => Instance.LogicalToDeviceUnitsScalingFactorY;


		static DpiHelper()
		{
			Instance = new DpiHelperImplementation();
		}
		public static System.Windows.Point LogicalToDeviceUnits(this System.Windows.Point logicalPoint)
		{
			return Instance.LogicalToDeviceUnits(logicalPoint);
		}
		public static Rect LogicalToDeviceUnits(this Rect logicalRect)
		{
			return Instance.LogicalToDeviceUnits(logicalRect);
		}

		public static System.Windows.Size LogicalToDeviceUnits(this System.Windows.Size logicalSize)
		{
			return Instance.LogicalToDeviceUnits(logicalSize);
		}

		public static System.Windows.Point DeviceToLogicalUnits(this System.Windows.Point devicePoint)
		{
			return Instance.DeviceToLogicalUnits(devicePoint);
		}

		public static Rect DeviceToLogicalUnits(this Rect deviceRect)
		{
			return Instance.DeviceToLogicalUnits(deviceRect);
		}

		public static System.Windows.Size DeviceToLogicalUnits(this System.Windows.Size deviceSize)
		{
			return Instance.DeviceToLogicalUnits(deviceSize);
		}

		public static Rect GetDeviceRect(this Window window)
		{
			return Instance.GetDeviceRect(window);
		}
	}
}
