using System;
using System.Timers;

namespace DotNetEtl.FileSystem
{
	public sealed class PollingFileWatcher : FileWatcher
	{
		private Timer pollingTimer;

		public PollingFileWatcher(IFileDataSourceFactory dataSourceFactory, string path, string filter = null)
			: base(dataSourceFactory, path, filter)
		{
		}

		public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(1);

		protected override void OnStart()
		{
			base.OnStart();

			this.CreatePollingTimer();
		}

		protected override void OnStop()
		{
			this.DestroyPollingTimer();

			base.OnStop();
		}

		private void CreatePollingTimer()
		{
			this.pollingTimer = new Timer(this.PollingInterval.TotalMilliseconds);
			this.pollingTimer.Elapsed += this.OnPoll;
			this.pollingTimer.Enabled = true;
		}

		private void DestroyPollingTimer()
		{
			this.pollingTimer.Enabled = false;
			this.pollingTimer.Dispose();
			this.pollingTimer = null;
		}

		private void OnPoll(object sender, ElapsedEventArgs e)
		{
			try
			{
				lock (this.SynchronizationObject)
				{
					if (this.State == DataSourceWatcherState.Started)
					{
						this.FindNewFiles();
					}
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
			}
		}
	}
}
