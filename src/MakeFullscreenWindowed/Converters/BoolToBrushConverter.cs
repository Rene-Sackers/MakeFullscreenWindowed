using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MakeFullscreenWindowed.Converters
{
	internal class BoolToBrushConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
			"SelectedBrush", typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(default(Brush)));

		public static readonly DependencyProperty DeselectedBrushProperty = DependencyProperty.Register(
			"DeselectedBrush", typeof(Brush), typeof(BoolToBrushConverter), new PropertyMetadata(default(Brush)));

		public Brush SelectedBrush
		{
			get => (Brush) GetValue(SelectedBrushProperty);
			set => SetValue(SelectedBrushProperty, value);
		}

		public Brush DeselectedBrush
		{
			get => (Brush) GetValue(DeselectedBrushProperty);
			set => SetValue(DeselectedBrushProperty, value);
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool)) return default(Brush);

			return (bool) value ? SelectedBrush : DeselectedBrush;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}