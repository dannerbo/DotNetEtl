using System;

namespace DotNetEtl
{
	public class CreateDataImportErrorEventArgs : ErrorEventArgs
	{
		public CreateDataImportErrorEventArgs(Exception exception, IDataSource dataSource)
			: base(exception)
		{
			this.DataSource = dataSource;
		}
		
		public IDataSource DataSource { get; private set; }
	}
}
