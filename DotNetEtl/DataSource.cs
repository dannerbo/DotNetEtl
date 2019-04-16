namespace DotNetEtl
{
	public class DataSource : IDataSource
	{
		public DataSource(IDataReader dataReader)
		{
			this.DataReader = dataReader;
		}

		public DataSource(IDataReaderFactory dataReaderFactory)
		{
			this.DataReaderFactory = dataReaderFactory;
		}

		protected IDataReader DataReader { get; private set; }
		protected IDataReaderFactory DataReaderFactory { get; private set; }

		public virtual IDataReader CreateDataReader()
		{
			return this.DataReader ?? this.DataReaderFactory.Create(this);
		}
	}
}
