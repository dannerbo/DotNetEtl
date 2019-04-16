using System;

namespace DotNetEtl
{
	public class DataImportFactory : IDataImportFactory
	{
		public DataImportFactory(Func<IDataSource, IDataImport> createDataImport)
		{
			this.CreateDataImport = createDataImport;
		}

		protected Func<IDataSource, IDataImport> CreateDataImport { get; private set; }

		public virtual IDataImport Create(IDataSource dataSource)
		{
			return this.CreateDataImport(dataSource);
		}
	}
	
	public class DataImportFactory<TDataImport> : DataImportFactory, IDataImportFactory<TDataImport>
		where TDataImport : IDataImport
	{
		public DataImportFactory(Func<IDataSource, TDataImport> createDataImport)
			: base(dataSource => createDataImport(dataSource))
		{
		}
	}
}
