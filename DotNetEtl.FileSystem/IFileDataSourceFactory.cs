namespace DotNetEtl.FileSystem
{
	public interface IFileDataSourceFactory
	{
		IFileDataSource Create(string filePath);
	}

	public interface IFileDataSourceFactory<TFileDataSource> : IFileDataSourceFactory
		where TFileDataSource : IFileDataSource
	{
	}
}
