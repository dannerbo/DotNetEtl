using System;
using System.Reflection;

namespace DotNetEtl
{
	public class SourceRecordFieldCountProvider : ISourceRecordFieldCountProvider
	{
		public bool TryGetSourceRecordFieldCount(Type recordType, object record, out int count)
		{
			var fieldCountAttribute = recordType.GetCustomAttribute<SourceRecordFieldCountAttribute>(true);

			count = fieldCountAttribute?.Count ?? -1;

			return fieldCountAttribute != null;
		}
	}
}
