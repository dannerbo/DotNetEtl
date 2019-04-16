using System.Collections.Generic;

namespace DotNetEtl
{
	public interface IDynamicRecordFieldProvider
	{
		IEnumerable<IDynamicRecordField> GetFields();
		IEnumerable<IDynamicRecordField> GetFields(object recordType);
	}
}
