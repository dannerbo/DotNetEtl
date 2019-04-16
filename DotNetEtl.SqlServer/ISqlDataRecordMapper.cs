using Microsoft.SqlServer.Server;

namespace DotNetEtl.SqlServer
{
	public interface ISqlDataRecordMapper
	{
		SqlDataRecord Map(object record);
	}
}
