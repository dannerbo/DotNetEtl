using System.Collections.Generic;

namespace DotNetEtl
{
	public class RecordEvaluatedEventArgs : RecordEventArgs
	{
		public RecordEvaluatedEventArgs(object record, bool wasSuccessful, IEnumerable<FieldFailure> failures)
			: base(record)
		{
			this.WasSuccessful = wasSuccessful;
			this.Failures = failures;
		}

		public bool WasSuccessful { get; private set; }
		public IEnumerable<FieldFailure> Failures { get; private set; }
	}
}
