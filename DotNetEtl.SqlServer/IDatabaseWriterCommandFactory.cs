using System.Data.SqlClient;

namespace DotNetEtl.SqlServer
{
	public interface IDatabaseWriterCommandFactory
	{
		SqlCommand Create(object record);
	}
}
