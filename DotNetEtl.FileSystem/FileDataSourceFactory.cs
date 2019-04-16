using System;

namespace DotNetEtl.FileSystem
{
	public class FileDataSourceFactory : IFileDataSourceFactory
	{
		public FileDataSourceFactory(Func<string, IFileDataSource> createDataSource)
		{
			this.CreateDataSource = createDataSource;
		}

		protected Func<string, IFileDataSource> CreateDataSource { get; private set; }

		public virtual IFileDataSource Create(string filePath)
		{
			return this.CreateDataSource(filePath);
		}
	}

	public class FileDataSourceFactory<TFileDataSource> : FileDataSourceFactory, IFileDataSourceFactory<TFileDataSource>
		where TFileDataSource : IFileDataSource
	{
		public FileDataSourceFactory(IDataReaderFactory dataReaderFactory)
			: base(filePath => CreateDataSourceWithActivator(filePath, dataReaderFactory))
		{
			this.DataReaderFactory = dataReaderFactory;
		}

		public FileDataSourceFactory(Func<string, TFileDataSource> createDataSource)
			: base(filePath => createDataSource(filePath))
		{
		}

		protected IDataReaderFactory DataReaderFactory { get; private set; }

		protected static TFileDataSource CreateDataSourceWithActivator(string filePath, IDataReaderFactory dataReaderFactory)
		{
			return (TFileDataSource)Activator.CreateInstance(typeof(TFileDataSource), dataReaderFactory, filePath);
		}
	}
}
