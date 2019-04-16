namespace DotNetEtl
{
	public interface IDataSource
	{
		IDataReader CreateDataReader();
	}
}
