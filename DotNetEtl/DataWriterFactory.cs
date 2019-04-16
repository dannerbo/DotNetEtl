using System;

namespace DotNetEtl
{
	public class DataWriterFactory : IDataWriterFactory
	{
		public DataWriterFactory(Func<IDataSource, IDataWriter> createDataWriter)
		{
			this.CreateDataWriter = createDataWriter;
		}

		protected Func<IDataSource, IDataWriter> CreateDataWriter { get; private set; }

		public virtual IDataWriter Create(IDataSource dataSource)
		{
			return this.CreateDataWriter(dataSource);
		}
	}

	public class DataWriterFactory<TDataWriter> : DataWriterFactory, IDataWriterFactory<TDataWriter>
		where TDataWriter : IDataWriter
	{
		public DataWriterFactory(Func<IDataSource, TDataWriter> createDataWriter)
			: base(dataSource => createDataWriter(dataSource))
		{
		}
	}
}
