using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using MakeFullscreenWindowed.Annotations;

namespace MakeFullscreenWindowed.UserControls
{
	public partial class Monitor : INotifyPropertyChanged
	{
		private string _monitorId;
		private bool _selected;

		public string MonitorId
		{
			get => _monitorId;
			private set
			{
				if (value == _monitorId) return;
				_monitorId = value;
				OnPropertyChanged();
			}
		}

		public bool Selected
		{
			get => _selected;
			set
			{
				if (value.Equals(_selected)) return;
				_selected = value;
				OnPropertyChanged();
			}
		}

		public Screen Screen { get; private set; }

		public Monitor(string monitorId, bool selected, Screen screen)
		{
			Screen = screen;
			_monitorId = monitorId;
			_selected = selected;

			InitializeComponent();
		}

		#region Property Change Notifier

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