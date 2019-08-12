using System.Collections.Generic;

namespace DotNetEtl
{
	public class RecordMappedEventArgs : RecordEvaluatedEventArgs
	{
		public RecordMappedEventArgs(int recordIndex, object record, bool wasSuccessful, IEnumerable<FieldFailure> failures, object mappedRecord)
			: base(recordIndex, record, wasSuccessful, failures)
		{
			this.MappedRecord = mappedRecord;
		}

		public object MappedRecord { get; private set; }
	}
}
