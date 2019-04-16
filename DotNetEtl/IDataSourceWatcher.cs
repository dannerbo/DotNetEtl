using System;

namespace DotNetEtl
{
	public interface IDataSourceWatcher
	{
		event EventHandler<DataSourceEventArgs> DataSourceAvailable;
		event EventHandler<ErrorEventArgs> Error;

		DataSourceWatcherState State { get; }
		
		void Start();
		void Stop();
	}
}
