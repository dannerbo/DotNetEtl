using System.Collections.Generic;

namespace DotNetEtl
{
	public class RecordMappedEventArgs : RecordEvaluatedEventArgs
	{
		public RecordMappedEventArgs(object record, bool wasSuccessful, IEnumerable<FieldFailure> failures, object mappedRecord)
			: base(record, wasSuccessful, failures)
		{
			this.MappedRecord = mappedRecord;
		}

		public object MappedRecord { get; private set; }
	}
}
