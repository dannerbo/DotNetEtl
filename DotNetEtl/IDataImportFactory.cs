namespace DotNetEtl
{
	public interface IDataImportFactory
	{
		IDataImport Create(IDataSource dataSource);
	}

	public interface IDataImportFactory<TDataImport> : IDataImportFactory
		where TDataImport : IDataImport
	{
	}
}
