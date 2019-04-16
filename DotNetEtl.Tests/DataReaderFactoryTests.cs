using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataReaderFactoryTests
	{
		[TestMethod]
		public void Create_CreateDataReader_DataReaderIsReturned()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();

			var createDataReader = new Func<IDataSource, IDataReader>(ds =>
			{
				Assert.AreEqual(dataSource, ds);

				return dataReader;
			});

			var dataReaderFactory = new DataReaderFactory(createDataReader);

			var returnedDataReader = dataReaderFactory.Create(dataSource);

			Assert.AreEqual(dataReader, returnedDataReader);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_CreateDataReaderThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();

			var createDataReader = new Func<IDataSource, IDataReader>(ds => throw new InternalTestFailureException());

			var dataReaderFactory = new DataReaderFactory(createDataReader);

			dataReaderFactory.Create(dataSource);
		}
	}
}
