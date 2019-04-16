using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DotNetEtl
{
	public interface IDynamicRecordField
	{
		string Name { get; }
		Type DotNetDataType { get; }
		int? SourceFieldOrdinal { get; }
		int? SourceFieldStartIndex { get; }
		int? SourceFieldLength { get; }
		string SourceFieldName { get; }
		int? DestinationFieldOrdinal { get; }
		int? DestinationFieldStartIndex { get; }
		int? DestinationFieldLength { get; }
		string DestinationFieldName { get; }

		IEnumerable<CustomAttributeBuilder> GetCustomAttributes();
	}
}
