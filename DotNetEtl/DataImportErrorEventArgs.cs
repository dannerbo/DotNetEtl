using System;

namespace DotNetEtl
{
	public class DataImportErrorEventArgs : ErrorEventArgs
	{
		public DataImportErrorEventArgs(Exception exception, IDataImport dataImport)
			: base(exception)
		{
			this.DataImport = dataImport;
		}

		public IDataImport DataImport { get; private set; }
	}
}
