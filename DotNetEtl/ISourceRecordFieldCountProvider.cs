using System;

namespace DotNetEtl
{
	public interface ISourceRecordFieldCountProvider
	{
		bool TryGetSourceRecordFieldCount(Type recordType, object record, out int count);
	}
}
