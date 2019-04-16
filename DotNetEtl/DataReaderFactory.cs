using System;

namespace DotNetEtl
{
	public class DataReaderFactory : IDataReaderFactory
	{
		public DataReaderFactory(Func<IDataSource, IDataReader> createDataReader)
		{
			this.CreateDataReader = createDataReader;
		}

		protected Func<IDataSource, IDataReader> CreateDataReader { get; private set; }

		public IDataReader Create(IDataSource dataSource)
		{
			return this.CreateDataReader(dataSource);
		}
	}

	public class DataReaderFactory<TDataReader> : DataReaderFactory, IDataReaderFactory<TDataReader>
		where TDataReader : IDataReader
	{
		public DataReaderFactory(Func<IDataSource, TDataReader> createDataReader)
			: base(dataSource => createDataReader(dataSource))
		{
		}
	}
}
