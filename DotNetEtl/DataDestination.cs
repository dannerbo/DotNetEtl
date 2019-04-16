namespace DotNetEtl
{
	public class DataDestination : IDataDestination
	{
		public DataDestination(IDataWriter dataWriter)
			: this(dataWriter, null)
		{
			this.DataWriter = dataWriter;
		}

		public DataDestination(IDataWriter dataWriter, IRecordFilter recordFilter)
		{
			this.DataWriter = dataWriter;
			this.RecordFilter = recordFilter;
		}

		public DataDestination(IDataWriterFactory dataWriterFactory)
			: this(dataWriterFactory, null)
		{
		}

		public DataDestination(IDataWriterFactory dataWriterFactory, IRecordFilter recordFilter)
		{
			this.DataWriterFactory = dataWriterFactory;
			this.RecordFilter = recordFilter;
		}

		public IRecordFilter RecordFilter { get; set; }
		protected IDataWriter DataWriter { get; private set; }
		protected IDataWriterFactory DataWriterFactory { get; set; }

		public virtual IDataWriter CreateDataWriter(IDataSource dataSource)
		{
			return this.DataWriter ?? this.DataWriterFactory.Create(dataSource);
		}
	}
}
