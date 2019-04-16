using System;
using System.Linq;
using System.Threading;

namespace DotNetEtl
{
	public abstract class DataSourceWatcher : IDataSourceWatcher
	{
		private CancellationTokenSource cancellationTokenSource;

		public event EventHandler<DataSourceEventArgs> DataSourceAvailable;
		public event EventHandler<ErrorEventArgs> Error;

		public DataSourceWatcherState State { get; private set; }

		protected object SynchronizationObject { get; private set; } = new object();
		protected CancellationToken CancellationToken => this.cancellationTokenSource.Token;

		protected abstract void OnStart();
		protected abstract void OnStop();

		public void Start()
		{
			lock (this.SynchronizationObject)
			{
				this.ThrowIfNotValidState(DataSourceWatcherState.Stopped);

				this.State = DataSourceWatcherState.Starting;

				this.cancellationTokenSource = new CancellationTokenSource();

				this.OnStart();

				this.State = DataSourceWatcherState.Started;
			}
		}

		public void Stop()
		{
			lock (this.SynchronizationObject)
			{
				this.ThrowIfNotValidState(DataSourceWatcherState.Started);

				this.State = DataSourceWatcherState.Stopping;

				this.cancellationTokenSource.Cancel();

				try
				{
					this.OnStop();
				}
				finally
				{
					this.cancellationTokenSource.Dispose();
					this.cancellationTokenSource = null;
				}

				this.State = DataSourceWatcherState.Stopped;
			}
		}

		protected virtual void OnDataSourceAvailable(IDataSource dataSource)
		{
			this.DataSourceAvailable?.Invoke(this, new DataSourceEventArgs(dataSource));
		}

		protected virtual void OnError(Exception exception, Func<ErrorEventArgs> createErrorEventArgs = null)
		{
			var eventArgs = createErrorEventArgs != null
				? createErrorEventArgs()
				: new ErrorEventArgs(exception);
			
			this.Error?.Invoke(this, eventArgs);
		}

		protected void ThrowIfNotValidState(params DataSourceWatcherState[] validStatuses)
		{
			if (!validStatuses.Contains(this.State))
			{
				throw new InvalidOperationException(
					$"{nameof(DataSourceWatcher)} is in an invalid state of '{this.State}' for the requested operation.");
			}
		}
	}
}
