namespace DotNetEtl
{
	public interface IDataReaderFactory
	{
		IDataReader Create(IDataSource dataSource);
	}

	public interface IDataReaderFactory<TDataReader> : IDataReaderFactory
		where TDataReader : IDataReader
	{
	}
}
