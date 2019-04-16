using System;
using System.IO;
using System.Timers;

namespace DotNetEtl.FileSystem
{
	public sealed class RealTimeFileWatcher : FileWatcher
	{
		private FileSystemWatcher fileSystemWatcher;
		private Timer resetTimer;

		public RealTimeFileWatcher(IFileDataSourceFactory dataSourceFactory, string path, string filter = null)
			: base(dataSourceFactory, path, filter)
		{
		}

		public TimeSpan ResetInterval { get; set; } = TimeSpan.FromMinutes(1);

		protected override void OnStart()
		{
			base.OnStart();

			this.CreateFileSystemWatcher();
			this.CreateResetTimer();

			this.fileSystemWatcher.EnableRaisingEvents = true;
			this.resetTimer.Enabled = true;
		}

		protected override void OnStop()
		{
			if (this.fileSystemWatcher != null)
			{
				this.fileSystemWatcher.EnableRaisingEvents = false;
			}

			this.resetTimer.Enabled = false;

			this.DestroyFileSystemWatcher();
			this.DestroyResetTimer();

			base.OnStop();
		}

		private void OnFileChanged(object sender, FileSystemEventArgs e)
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

		private void OnReset(object sender, ElapsedEventArgs e)
		{
			try
			{
				lock (this.SynchronizationObject)
				{
					if (this.State == DataSourceWatcherState.Started)
					{
						if (this.fileSystemWatcher != null)
						{
							this.fileSystemWatcher.EnableRaisingEvents = false;
						}

						this.DestroyFileSystemWatcher();
						this.CreateFileSystemWatcher();

						this.fileSystemWatcher.EnableRaisingEvents = true;

						this.FindNewFiles();
					}
				}
			}
			catch (Exception exception)
			{
				this.OnError(exception);
			}
		}

		private void OnFileSystemWatcherError(object sender, System.IO.ErrorEventArgs e)
		{
			this.OnError(e.GetException());
		}

		private void CreateFileSystemWatcher()
		{
			this.fileSystemWatcher = this.Filter != null
				? new FileSystemWatcher(this.Path, this.Filter)
				: new FileSystemWatcher(this.Path);

			this.fileSystemWatcher.NotifyFilter = NotifyFilters.FileName;
			this.fileSystemWatcher.Error += this.OnFileSystemWatcherError;
			this.fileSystemWatcher.Created += this.OnFileChanged;
			this.fileSystemWatcher.Renamed += this.OnFileChanged;
			this.fileSystemWatcher.Deleted += this.OnFileChanged;
		}

		private void CreateResetTimer()
		{
			this.resetTimer = new Timer(this.ResetInterval.TotalMilliseconds);
			this.resetTimer.AutoReset = true;
			this.resetTimer.Elapsed += this.OnReset;
		}

		private void DestroyFileSystemWatcher()
		{
			if (this.fileSystemWatcher != null)
			{
				this.fileSystemWatcher.Error -= this.OnFileSystemWatcherError;
				this.fileSystemWatcher.Created -= this.OnFileChanged;
				this.fileSystemWatcher.Renamed -= this.OnFileChanged;
				this.fileSystemWatcher.Deleted -= this.OnFileChanged;
				this.fileSystemWatcher.Dispose();
				this.fileSystemWatcher = null;
			}
		}
		
		private void DestroyResetTimer()
		{
			if (this.resetTimer != null)
			{
				this.resetTimer.Elapsed -= this.OnReset;
				this.resetTimer.Dispose();
				this.resetTimer = null;
			}
		}
	}
}
