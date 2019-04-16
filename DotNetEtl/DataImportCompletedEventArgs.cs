using System.Collections.Generic;

namespace DotNetEtl
{
	public class DataImportCompletedEventArgs : DataImportEventArgs
	{
		public DataImportCompletedEventArgs(IDataImport dataImport, bool wasSuccessful, IEnumerable<RecordFailure> failures)
			: base(dataImport)
		{
			this.WasSuccessful = wasSuccessful;
			this.Failures = failures;
		}

		public bool WasSuccessful { get; private set; }
		public IEnumerable<RecordFailure> Failures { get; private set; }
	}
}
