using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DotNetEtl.FileSystem
{
	public abstract class FileWatcher : DataSourceWatcher, IFileWatcher
	{
		private IEnumerable<string> files;
		private Task watchingTask;
		private ManualResetEventSlim findNewFilesSignal;
		private ActionBlock<string> monitorLockedFilesActionBlock;

		public FileWatcher(IFileDataSourceFactory dataSourceFactory, string path, string filter = null)
		{
			this.DataSourceFactory = dataSourceFactory;
			this.Path = path;
			this.Filter = filter;
		}

		public string Path { get; set; }
		public string Filter { get; set; }
		public bool ShouldIgnoreExistingFiles { get; set; }
		protected IFileDataSourceFactory DataSourceFactory { get; private set; }

		protected override void OnStart()
		{
			this.watchingTask = new Task(this.OnWatching);
			this.findNewFilesSignal = new ManualResetEventSlim(true);
			this.monitorLockedFilesActionBlock = new ActionBlock<string>(
				filePath => this.MonitorLockedFile(filePath),
				new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded });

			this.watchingTask.Start();
		}

		protected override void OnStop()
		{
			try
			{
				this.monitorLockedFilesActionBlock.Complete();
				
				ActionHelper.PerformCancelableAction(() => this.watchingTask.Wait());
				ActionHelper.PerformCancelableAction(() => this.monitorLockedFilesActionBlock.Completion.Wait());
			}
			finally
			{
				this.watchingTask.Dispose();
				this.findNewFilesSignal.Dispose();

				this.watchingTask = null;
				this.findNewFilesSignal = null;
				this.monitorLockedFilesActionBlock = null;
				this.files = null;
			}
		}

		protected void FindNewFiles()
		{
			this.findNewFilesSignal.Set();
		}

		protected virtual IEnumerable<string> GetFiles()
		{
			return this.Filter != null
				? Directory.GetFiles(this.Path, this.Filter)
				: Directory.GetFiles(this.Path);
		}

		protected virtual IEnumerable<string> GetNewFiles()
		{
			IEnumerable<string> newFiles;

			var files = this.GetFiles();

			if (this.files == null)
			{
				newFiles = this.ShouldIgnoreExistingFiles
					? Array.Empty<string>()
					: files;
			}
			else
			{
				newFiles = files.Except(this.files);
			}

			this.files = files;

			var lockedFiles = this.GetLockedFiles(newFiles);

			if (lockedFiles.Count() > 0)
			{
				this.MonitorLockedFiles(lockedFiles);
			}

			return newFiles.Except(lockedFiles);
		}

		private IEnumerable<string> GetLockedFiles(IEnumerable<string> files)
		{
			var lockedFiles = new List<string>();

			foreach (var filePath in files)
			{
				if (FileHelper.IsFileLocked(filePath))
				{
					lockedFiles.Add(filePath);
				}
			}

			return lockedFiles;
		}

		private void MonitorLockedFiles(IEnumerable<string> lockedFiles)
		{
			foreach (var file in lockedFiles)
			{
				this.monitorLockedFilesActionBlock.Post(file);
			}
		}

		private void MonitorLockedFile(string filePath)
		{
			while (true)
			{
				this.CancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
				this.CancellationToken.ThrowIfCancellationRequested();

				try
				{
					if (!FileHelper.IsFileLocked(filePath))
					{
						this.OnFileFound(filePath);

						return;
					}
				}
				catch (Exception exception)
				{
					this.OnError(exception, () => new FileWatcherErrorEventArgs(exception, filePath));

					return;
				}
			}
		}

		private void OnWatching()
		{
			while (true)
			{
				this.findNewFilesSignal.Wait(this.CancellationToken);
				this.findNewFilesSignal.Reset();

				try
				{
					foreach (var filePath in this.GetNewFiles())
					{
						this.CancellationToken.ThrowIfCancellationRequested();

						try
						{
							this.OnFileFound(filePath);
						}
						catch (Exception exception)
						{
							this.OnError(exception, () => new FileWatcherErrorEventArgs(exception, filePath));
						}
					}
				}
				catch (Exception exception)
				{
					this.OnError(exception);
				}
			}
		}
		
		private void OnFileFound(string filePath)
		{
			var dataSource = this.DataSourceFactory.Create(filePath);

			this.OnDataSourceAvailable(dataSource);
		}
	}
}
