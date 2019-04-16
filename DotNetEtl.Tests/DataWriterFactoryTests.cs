using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataWriterFactoryTests
	{
		[TestMethod]
		public void DataWriterFactoryTest()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			var createDataWriter = new Func<IDataSource, IDataWriter>(ds =>
			{
				Assert.AreEqual(dataSource, ds);

				return dataWriter;
			});

			var dataWriterFactory = new DataWriterFactory(createDataWriter);

			var returnedDataWriter = dataWriterFactory.Create(dataSource);

			Assert.AreEqual(dataWriter, returnedDataWriter);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_CreateDataReaderThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			var createDataWriter = new Func<IDataSource, IDataWriter>(ds => throw new InternalTestFailureException());

			var dataWriterFactory = new DataWriterFactory(createDataWriter);

			dataWriterFactory.Create(dataSource);
		}
	}
}
