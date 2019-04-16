using System;
using System.Collections.Generic;

namespace DotNetEtl
{
	public interface IRecordValidator
	{
		bool TryValidate(object record, out IEnumerable<FieldFailure> failures);
	}
}
