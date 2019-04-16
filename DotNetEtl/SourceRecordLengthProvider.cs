using System;
using System.Reflection;

namespace DotNetEtl
{
	public class SourceRecordLengthProvider : ISourceRecordLengthProvider
	{
		public bool TryGetSourceRecordLength(Type recordType, object record, out int length)
		{
			var recordLengthAttribute = recordType.GetCustomAttribute<SourceRecordLengthAttribute>(true);

			length = recordLengthAttribute?.Length ?? -1;

			return recordLengthAttribute != null;
		}
	}
}
