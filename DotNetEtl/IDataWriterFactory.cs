namespace DotNetEtl
{
	public interface IDataWriterFactory
	{
		IDataWriter Create(IDataSource dataSource);
	}

	public interface IDataWriterFactory<TDataWriter> : IDataWriterFactory
		where TDataWriter : IDataWriter
	{
	}
}
