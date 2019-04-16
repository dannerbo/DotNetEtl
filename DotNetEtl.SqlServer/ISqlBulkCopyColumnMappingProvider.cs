using System.Collections.Generic;
using System.Data.SqlClient;

namespace DotNetEtl.SqlServer
{
	public interface ISqlBulkCopyColumnMappingProvider
	{
		IEnumerable<SqlBulkCopyColumnMapping> GetColumnMappings();
	}
}
