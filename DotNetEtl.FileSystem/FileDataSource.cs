namespace DotNetEtl.FileSystem
{
	public class FileDataSource : DataSource, IFileDataSource
	{
		public FileDataSource(IDataReader dataReader, string filePath)
			: base(dataReader)
		{
			this.FilePath = filePath;
		}

		public FileDataSource(IDataReaderFactory dataReaderFactory, string filePath)
			: base(dataReaderFactory)
		{
			this.FilePath = filePath;
		}

		public string FilePath { get; private set; }
	}
}
