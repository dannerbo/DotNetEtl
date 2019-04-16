using System.Collections.Generic;

namespace DotNetEtl
{
	public interface IRecordExpander
	{
		IEnumerable<object> Expand(object record);
	}
}
