using System;
using System.Collections.Generic;

namespace DotNetEtl
{
	public interface IDataReader : IDisposable
	{
		void Open();
		void Close();
		bool TryReadRecord(out object record, out IEnumerable<FieldFailure> failures);
	}
}
