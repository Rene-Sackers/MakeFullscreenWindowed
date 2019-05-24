using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MakeFullscreenWindowed.UserControls
{
	public partial class MonitorPicker
	{
		public Monitor SelectedMonitor
		{
			get => _selectedMonitor;
			private set
			{
				if (_monitors != null)
					foreach (var m in _monitors.Where(m => !Equals(m, value)))
						m.Selected = false;
				value.Selected = true;

				_selectedMonitor = value;
			}
		}

		private List<Monitor> _monitors;
		private Monitor _selectedMonitor;

		public MonitorPicker()
		{
			InitializeComponent();

			LoadMonitors();
		}

		private void LoadMonitors()
		{
			const int sizeScaling = 20;

			CanvasMonitors.Children.Clear();
			_monitors = new List<Monitor>();

			var allScreens = Screen.AllScreens;

			var mostLeft = (double) allScreens.Select(s => s.Bounds.Left).Min();
			var mostTop = (double) allScreens.Select(s => s.Bounds.Top).Min();
			var mostRight = (double) allScreens.Select(s => s.Bounds.Right).Max();
			var mostBottom = (double) allScreens.Select(s => s.Bounds.Bottom).Max();

			var leftDifference = 0 - mostLeft;
			var topDifference = 0 - mostTop;

			var screenId = 0;
			foreach (var screen in allScreens)
			{
				screenId++;


				var monitor = new Monitor(screenId + "", screen.Primary, screen)
				{
					Width = screen.Bounds.Width / sizeScaling,
					Height = screen.Bounds.Height / sizeScaling
				};
				if (monitor.Selected) SelectedMonitor = monitor;

				monitor.MouseLeftButtonUp += OnMouseLeftButtonUp;
				_monitors.Add(monitor);

				Canvas.SetLeft(monitor, (screen.Bounds.Left + leftDifference) / sizeScaling);
				Canvas.SetTop(monitor, (screen.Bounds.Top + topDifference) / sizeScaling);
				CanvasMonitors.Children.Add(monitor);
			}

			CanvasMonitors.Width = (mostRight - mostLeft) / sizeScaling;
			CanvasMonitors.Height = (mostBottom - mostTop) / sizeScaling;
		}

		private void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			SelectedMonitor = (Monitor) sender;
		}
	}
}