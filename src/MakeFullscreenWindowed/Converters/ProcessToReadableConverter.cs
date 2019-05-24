using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace MakeFullscreenWindowed.Converters
{
	internal class ProcessToReadableConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var processes = value as IEnumerable<Process>;
			return processes == null ? null : processes.Select(CreateListItem);
		}

		private static Grid CreateListItem(Process process)
		{
			var grid = new Grid {DataContext = process};
			grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(16)});
			grid.ColumnDefinitions.Add(new ColumnDefinition());

			Icon icon;
			try
			{
				icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);
			}
			catch
			{
				icon = null;
			}

			var image = new Image {Width = 16, Height = 16, VerticalAlignment = VerticalAlignment.Center};
			if (icon != null) image.Source = icon.ToImageSource();
			Grid.SetColumn(image, 0);
			grid.Children.Add(image);

			var textBlock = new TextBlock
			{
				Text = process.MainWindowTitle == ""
					? process.ProcessName
					: process.ProcessName + " - " + process.MainWindowTitle,
				VerticalAlignment = VerticalAlignment.Center,
				Margin = new Thickness(5, 0, 0, 0)
			};
			Grid.SetColumn(textBlock, 1);
			grid.Children.Add(textBlock);

			return grid;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	internal static class IconUtilities
	{
		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		public static ImageSource ToImageSource(this Icon icon)
		{
			var bitmap = icon.ToBitmap();
			var hBitmap = bitmap.GetHbitmap();

			var wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap,
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());

			if (!DeleteObject(hBitmap)) throw new Win32Exception();

			return wpfBitmap;
		}
	}
}