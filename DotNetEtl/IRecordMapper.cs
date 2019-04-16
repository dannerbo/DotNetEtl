using System;
using System.Collections.Generic;

namespace DotNetEtl
{
	public interface IRecordMapper
	{
		bool TryMap(object source, out object record, out IEnumerable<FieldFailure> failures);
	}
}
