using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using MakeFullscreenWindowed.Annotations;

namespace MakeFullscreenWindowed
{
	public partial class MainWindow : INotifyPropertyChanged
	{
		#region Imports

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		#endregion

		public bool StretchWindow { get; set; }

		public IEnumerable<Process> Processes
		{
			get { return Process.GetProcesses().Where(p => p.MainWindowHandle != IntPtr.Zero); }
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		private void PlaceButtonClicked(object sender, RoutedEventArgs e)
		{
			var selectedItem = ListBoxProcesses.SelectedItem as Grid;
			if (selectedItem == null) return;

			var process = selectedItem.DataContext as Process;
			if (process == null) return;

			if (process.HasExited)
			{
				MessageBox.Show("Process has closed down. Can't place.");
				OnPropertyChanged("Processes");
				return;
			}

			if (process.MainWindowHandle == IntPtr.Zero)
			{
				MessageBox.Show("Can't find the main window of the process.");
				OnPropertyChanged("Processes");
				return;
			}

			if (MonitorPickerPicker.SelectedMonitor == null)
			{
				MessageBox.Show("Please select a monitor.");
				return;
			}

			var selectedMonitor = MonitorPickerPicker.SelectedMonitor.Screen;

			SetWindowLong(process.MainWindowHandle, -16, 0x10000000 | 0x800000);

			SendMessage(process.MainWindowHandle, 0x231, IntPtr.Zero, IntPtr.Zero); // WM_ENTERSIZEMOVE

			var stretchWindowTopAndLeft = StretchWindow ? -1 : 0;
			var stretchWindowRightAndBottom = StretchWindow ? 2 : 0;

			SetWindowPos(
				process.MainWindowHandle,
				new IntPtr(0),
				selectedMonitor.Bounds.Left + stretchWindowTopAndLeft,
				selectedMonitor.Bounds.Top + stretchWindowTopAndLeft,
				selectedMonitor.Bounds.Width + stretchWindowRightAndBottom,
				selectedMonitor.Bounds.Height + stretchWindowRightAndBottom,
				0x20
			);

			SendMessage(process.MainWindowHandle, 0x232, IntPtr.Zero, IntPtr.Zero); // WM_EXITSIZEMOVE
		}

		private void RefreshClicked(object sender, RoutedEventArgs e)
		{
			OnPropertyChanged("Processes");
		}

		#region Property Change Notifyer

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}