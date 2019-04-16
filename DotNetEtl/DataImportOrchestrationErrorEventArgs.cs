using System;

namespace DotNetEtl
{
	public class DataImportOrchestrationErrorEventArgs : ErrorEventArgs
	{
		public DataImportOrchestrationErrorEventArgs(Exception exception, IDataSource dataSource = null, IDataImport dataImport = null)
			: base(exception)
		{
			this.DataSource = dataSource;
			this.DataImport = dataImport;
		}

		public IDataSource DataSource { get; private set; }
		public IDataImport DataImport { get; private set; }
	}
}
