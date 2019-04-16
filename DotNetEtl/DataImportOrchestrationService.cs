using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DotNetEtl
{
	public class DataImportOrchestrationService : IDataImportOrchestrationService
	{
		private ActionBlock<IDataSource> dataImportQueue;
		private CountdownEvent completionSignal;
		
		public DataImportOrchestrationService(IDataSourceWatcher dataSourceWatcher, IDataImportFactory dataImportFactory)
			: this(new IDataSourceWatcher[] { dataSourceWatcher }, dataImportFactory)
		{
		}

		public DataImportOrchestrationService(IEnumerable<IDataSourceWatcher> dataSourceWatchers, IDataImportFactory dataImportFactory)
		{
			this.DataSourceWatchers = dataSourceWatchers;
			this.DataImportFactory = dataImportFactory;
		}

		public event EventHandler<DataImportOrchestrationErrorEventArgs> Error;
		public event EventHandler<DataSourceEventArgs> DataImportPrevented;
		public event EventHandler<CreateDataImportErrorEventArgs> CreateDataImportError;
		public event EventHandler<DataImportEventArgs> DataImportStarted;
		public event EventHandler<DataImportCompletedEventArgs> DataImportCompleted;
		public event EventHandler<DataImportEventArgs> DataImportCanceled;
		public event EventHandler<DataImportErrorEventArgs> DataImportError;

		public bool UseInternalQueue { get; set; } = true;
		public int MaxDegreeOfParallelismForInternalQueue { get; set; } = DataflowBlockOptions.Unbounded;
		public DataImportOrchestrationServiceState State { get; private set; }

		protected object SynchronizationObject { get; private set; } = new object();
		protected IEnumerable<IDataSourceWatcher> DataSourceWatchers { get; private set; }
		protected IDataImportFactory DataImportFactory { get; private set; }
		protected CancellationTokenSource CancellationTokenSource { get; private set; }
		protected CancellationToken CancellationToken => this.CancellationTokenSource.Token;
		
		public void Start()
		{
			lock (this.SynchronizationObject)
			{
				this.ThrowIfNotValidState(DataImportOrchestrationServiceState.Stopped);

				this.State = DataImportOrchestrationServiceState.Starting;

				this.CancellationTokenSource = new CancellationTokenSource();

				if (this.UseInternalQueue)
				{
					this.dataImportQueue = new ActionBlock<IDataSource>(
						dataSource => this.CreateAndRunDataImport(dataSource),
						new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelismForInternalQueue });
				}
				else
				{
					this.completionSignal = new CountdownEvent(1);
				}

				this.OnStarting();

				Parallel.ForEach(this.DataSourceWatchers, dataSourceWatcher =>
				{
					dataSourceWatcher.DataSourceAvailable += this.OnDataSourceAvailable;
					dataSourceWatcher.Start();
				});

				this.State = DataImportOrchestrationServiceState.Started;
			}
		}

		public void Stop()
		{
			lock (this.SynchronizationObject)
			{
				this.ThrowIfNotValidState(DataImportOrchestrationServiceState.Started);

				this.State = DataImportOrchestrationServiceState.Stopping;

				try
				{
					this.CancellationTokenSource.Cancel();

					Parallel.ForEach(this.DataSourceWatchers, dataSourceWatcher =>
					{
						dataSourceWatcher.DataSourceAvailable -= this.OnDataSourceAvailable;
						dataSourceWatcher.Stop();
					});

					this.OnStopping();

					if (this.UseInternalQueue)
					{
						this.dataImportQueue.Complete();
						ActionHelper.PerformCancelableAction(() => this.dataImportQueue.Completion.Wait());
					}
					else
					{
						this.completionSignal.Signal();
						this.completionSignal.Wait();
					}
				}
				finally
				{
					this.dataImportQueue = null;
					
					if (this.completionSignal != null)
					{
						this.completionSignal.Dispose();
						this.completionSignal = null;
					}

					this.CancellationTokenSource.Dispose();
					this.CancellationTokenSource = null;
				}

				this.State = DataImportOrchestrationServiceState.Stopped;
			}
		}

		protected virtual void OnStarting()
		{
		}

		protected virtual void OnStopping()
		{
		}

		protected virtual void OnDataSourceAvailable(IDataSource dataSource)
		{
			if (this.UseInternalQueue)
			{
				this.dataImportQueue.Post(dataSource);
			}
			else
			{
				this.completionSignal.AddCount();

				try
				{
					this.CreateAndRunDataImport(dataSource);
				}
				finally
				{
					this.completionSignal.Signal();
				}
			}
		}

		protected virtual IDataImport CreateDataImport(IDataSource dataSource)
		{
			return this.DataImportFactory.Create(dataSource);
		}

		protected virtual bool TryRunDataImport(IDataImport dataImport, out IEnumerable<RecordFailure> failures)
		{
			return dataImport.TryRun(this.CancellationToken, out failures);
		}

		protected virtual void OnDataImportPrevented(IDataSource dataSource)
		{
			this.DataImportPrevented?.Invoke(this, new DataSourceEventArgs(dataSource));
		}
		
		protected virtual void OnCreateDataImportError(IDataSource dataSource, Exception exception)
		{
			this.CreateDataImportError?.Invoke(this, new CreateDataImportErrorEventArgs(exception, dataSource));
		}

		protected virtual void OnDataImportStarted(IDataImport dataImport)
		{
			this.DataImportStarted?.Invoke(this, new DataImportEventArgs(dataImport));
		}

		protected virtual void OnDataImportCanceled(IDataImport dataImport)
		{
			this.DataImportCanceled?.Invoke(this, new DataImportEventArgs(dataImport));
		}

		protected virtual void OnDataImportCompleted(IDataImport dataImport, bool wasSuccessful, IEnumerable<RecordFailure> failures)
		{
			this.DataImportCompleted?.Invoke(this, new DataImportCompletedEventArgs(dataImport, wasSuccessful, failures));
		}

		protected virtual void OnDataImportError(IDataImport dataImport, Exception exception)
		{
			this.DataImportError?.Invoke(this, new DataImportErrorEventArgs(exception, dataImport));
		}

		private void CreateAndRunDataImport(IDataSource dataSource)
		{
			if (this.CancellationToken.IsCancellationRequested)
			{
				this.OnDataImportPrevented(dataSource);

				return;
			}

			IDataImport dataImport = null;
			
			try
			{
				try
				{
					dataImport = this.CreateDataImport(dataSource);
				}
				catch (Exception exception)
				{
					this.OnCreateDataImportError(dataSource, exception);

					return;
				}

				this.OnDataImportStarted(dataImport);

				bool wasSuccessful;
				IEnumerable<RecordFailure> failures;

				try
				{
					wasSuccessful = this.TryRunDataImport(dataImport, out failures);
				}
				catch (OperationCanceledException)
				{
					this.OnDataImportCanceled(dataImport);

					return;
				}
				catch (Exception exception)
				{
					this.OnDataImportError(dataImport, exception);

					return;
				}

				this.OnDataImportCompleted(dataImport, wasSuccessful, failures);
			}
			catch (Exception exception)
			{
				this.OnError(exception, dataSource, dataImport);
			}
		}

		protected virtual void OnError(Exception exception, IDataSource dataSource, IDataImport dataImport)
		{
			this.Error?.Invoke(this, new DataImportOrchestrationErrorEventArgs(exception, dataSource, dataImport));
		}

		private void ThrowIfNotValidState(params DataImportOrchestrationServiceState[] validStatuses)
		{
			if (!validStatuses.Contains(this.State))
			{
				throw new InvalidOperationException(
					$"{nameof(DataImportOrchestrationService)} is in an invalid state of '{this.State}' for the requested operation.");
			}
		}

		private void OnDataSourceAvailable(object sender, DataSourceEventArgs e)
		{
			this.OnDataSourceAvailable(e.DataSource);
		}
	}
}
