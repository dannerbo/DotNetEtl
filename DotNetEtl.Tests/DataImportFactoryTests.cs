using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportFactoryTests
	{
		[TestMethod]
		public void Create_DataSourceIsProvided_ExpectedDataImportIsReturned()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();
			var createDataImport = new Func<IDataSource, IDataImport>(ds => ds.Equals(dataSource) ? dataImport : null);

			var dataImportFactory = new DataImportFactory(createDataImport);

			var returnedDataImport = dataImportFactory.Create(dataSource);

			Assert.AreEqual(dataImport, returnedDataImport);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_CreateDataImportFuncThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();
			var createDataImport = new Func<IDataSource, IDataImport>(ds => throw new InternalTestFailureException());

			var dataImportFactory = new DataImportFactory(createDataImport);

			dataImportFactory.Create(dataSource);
		}
	}
}
