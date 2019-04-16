using System;

namespace DotNetEtl
{
	public interface IDataImportOrchestrationService
	{
		event EventHandler<DataImportOrchestrationErrorEventArgs> Error;
		event EventHandler<DataSourceEventArgs> DataImportPrevented;
		event EventHandler<CreateDataImportErrorEventArgs> CreateDataImportError;
		event EventHandler<DataImportEventArgs> DataImportStarted;
		event EventHandler<DataImportCompletedEventArgs> DataImportCompleted;
		event EventHandler<DataImportEventArgs> DataImportCanceled;
		event EventHandler<DataImportErrorEventArgs> DataImportError;

		DataImportOrchestrationServiceState State { get; }

		void Start();
		void Stop();
	}
}
