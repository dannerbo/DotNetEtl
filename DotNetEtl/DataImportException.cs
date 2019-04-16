using System;
using System.Runtime.Serialization;

namespace DotNetEtl
{
	[Serializable]
	public class DataImportException : Exception
	{
		public DataImportException(IDataImport dataImport)
		{
			this.DataImport = dataImport;
		}

		public DataImportException(IDataImport dataImport, string message)
			: base(message)
		{
			this.DataImport = dataImport;
		}

		public DataImportException(IDataImport dataImport, string message, Exception innerException)
			: base(message, innerException)
		{
			this.DataImport = dataImport;
		}

		protected DataImportException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public IDataImport DataImport { get; private set; }
	}
}
