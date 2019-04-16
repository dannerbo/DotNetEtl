using System.Collections.Generic;
using System.Data.SqlClient;

namespace DotNetEtl.SqlServer
{
	public interface IDatabaseWriterCommandParameterProvider
	{
		IEnumerable<SqlParameter> GetParameters(object record);
	}
}
