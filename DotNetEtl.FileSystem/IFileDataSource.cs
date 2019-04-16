namespace DotNetEtl.FileSystem
{
	public interface IFileDataSource : IDataSource
	{
		string FilePath { get; }
	}
}
