using System;

namespace DotNetEtl
{
	public class DataImportEventArgs : EventArgs
	{
		public DataImportEventArgs(IDataImport dataImport)
		{
			this.DataImport = dataImport;
		}
		
		public IDataImport DataImport { get; private set; }
	}
}
