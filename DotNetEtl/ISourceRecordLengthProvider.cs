using System;

namespace DotNetEtl
{
	public interface ISourceRecordLengthProvider
	{
		bool TryGetSourceRecordLength(Type recordType, object record, out int length);
	}
}
