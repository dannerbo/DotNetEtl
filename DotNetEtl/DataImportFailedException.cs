using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DotNetEtl
{
	[Serializable]
	public class DataImportFailedException : Exception
	{
		public DataImportFailedException(IEnumerable<RecordFailure> failures)
		{
			this.Failures = failures;
		}

		public DataImportFailedException(IEnumerable<RecordFailure> failures, string message)
			: base(message)
		{
			this.Failures = failures;
		}

		public DataImportFailedException(IEnumerable<RecordFailure> failures, string message, Exception innerException)
			: base(message, innerException)
		{
			this.Failures = failures;
		}

		protected DataImportFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public IEnumerable<RecordFailure> Failures { get; private set; }
	}
}
