using System.Collections.Generic;

namespace DotNetEtl
{
	public class RecordFailure
	{
		public RecordFailure(int recordIndex, IEnumerable<FieldFailure> fieldFailures)
		{
			this.RecordIndex = recordIndex;
			this.FieldFailures = fieldFailures;
		}

		public RecordFailure()
		{
		}

		public int RecordIndex { get; set; }
		public IEnumerable<FieldFailure> FieldFailures { get; set; }
	}
}
