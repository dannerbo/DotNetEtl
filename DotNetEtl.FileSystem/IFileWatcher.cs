namespace DotNetEtl.FileSystem
{
	public interface IFileWatcher : IDataSourceWatcher
	{
		string Path { get; set; }
		string Filter { get; set; }
		bool ShouldIgnoreExistingFiles { get; set; }
	}
}
