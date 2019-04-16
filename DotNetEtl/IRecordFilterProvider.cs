using System.Collections.Generic;

namespace DotNetEtl
{
	public interface IRecordFilterProvider
	{
		IEnumerable<IRecordFilter> GetRecordFilters();
	}
}
