using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataDestinationTests
	{
		[TestMethod]
		public void RecordFilter_SetRecordFilterProperty_RecordFilterPropertyIsSet()
		{
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordFilter = MockRepository.GenerateMock<IRecordFilter>();

			var dataDestination = new DataDestination(dataWriter);

			dataDestination.RecordFilter = recordFilter;

			Assert.AreEqual(recordFilter, dataDestination.RecordFilter);
		}

		[TestMethod]
		public void CreateDataWriter_DataWriterProvided_ProvidedDataWriterIsReturned()
		{
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();

			var dataDestination = new DataDestination(dataWriter);

			var returnedDataWriter = dataDestination.CreateDataWriter(dataSource);

			Assert.AreEqual(dataWriter, returnedDataWriter);
		}

		[TestMethod]
		public void CreateDataWriter_DataWriterAndRecordFilterProvided_ProvidedDataWriterIsReturnedAndRecordFilterPropertyIsSet()
		{
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordFilter = MockRepository.GenerateMock<IRecordFilter>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();

			var dataDestination = new DataDestination(dataWriter, recordFilter);

			var returnedDataWriter = dataDestination.CreateDataWriter(dataSource);

			Assert.AreEqual(dataWriter, returnedDataWriter);
			Assert.AreEqual(recordFilter, dataDestination.RecordFilter);
		}

		[TestMethod]
		public void CreateDataWriter_DataWriterFactoryAndRecordFilterProvided_ExpectedDataWriterIsReturnedAndRecordFilterPropertyIsSet()
		{
			var dataWriterFactory = MockRepository.GenerateMock<IDataWriterFactory>();
			var recordFilter = MockRepository.GenerateMock<IRecordFilter>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataWriterFactory.Expect(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();

			var dataDestination = new DataDestination(dataWriterFactory, recordFilter);

			var returnedDataWriter = dataDestination.CreateDataWriter(dataSource);

			dataWriterFactory.VerifyAllExpectations();

			Assert.AreEqual(dataWriter, returnedDataWriter);
			Assert.AreEqual(recordFilter, dataDestination.RecordFilter);
		}

		[TestMethod]
		public void CreateDataWriter_DataWriterFactoryProvided_ExpectedDataWriterIsReturned()
		{
			var dataWriterFactory = MockRepository.GenerateMock<IDataWriterFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataWriterFactory.Expect(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();

			var dataDestination = new DataDestination(dataWriterFactory);

			var returnedDataWriter = dataDestination.CreateDataWriter(dataSource);

			dataWriterFactory.VerifyAllExpectations();

			Assert.AreEqual(dataWriter, returnedDataWriter);
		}
	}
}
