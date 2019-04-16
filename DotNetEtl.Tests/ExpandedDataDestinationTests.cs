using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class ExpandedDataDestinationTests
	{
		[TestMethod]
		public void CreateDataWriter_OneDataDestination_ExpandedDataWriterIsCreated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();

			var expandedDataDestination = new ExpandedDataDestination(recordExpander, dataDestination)
			{
				MaxDegreeOfParallelismForExpandedRecords = 2,
				MaxDegreeOfParallelismForDataWriters = 3
			};

			var returnedDataWriter = expandedDataDestination.CreateDataWriter(dataSource);

			var expandedDataWriter = returnedDataWriter as ExpandedDataWriter;

			dataDestination.VerifyAllExpectations();

			Assert.IsNotNull(expandedDataWriter);
			Assert.AreEqual(expandedDataDestination.MaxDegreeOfParallelismForExpandedRecords, expandedDataWriter.MaxDegreeOfParallelismForExpandedRecords);
			Assert.AreEqual(expandedDataDestination.MaxDegreeOfParallelismForDataWriters, expandedDataWriter.MaxDegreeOfParallelismForDataWriters);
		}

		[TestMethod]
		public void ExpandedDataDestination_OneDataDestinationAndOneExpandedRecord_ExpandedRecordIsWritten()
		{
			var records = new List<object>()
			{
				new object()
			};
			var expandedRecords = new List<object>()
			{
				new object()
			};

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			recordExpander.Expect(x => x.Expand(Arg<object>.Is.Equal(records[0]))).Return(expandedRecords).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();

			var expandedDataDestination = new ExpandedDataDestination(recordExpander, dataDestination);

			var returnedDataWriter = expandedDataDestination.CreateDataWriter(dataSource);

			returnedDataWriter.WriteRecord(records[0]);

			dataDestination.VerifyAllExpectations();
			recordExpander.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();
		}

		[TestMethod]
		public void ExpandedDataDestination_OneDataDestinationAndMultipleExpandedRecords_ExpandedRecordsAreWritten()
		{
			var records = new List<object>()
			{
				new object()
			};
			var expandedRecords = new List<object>()
			{
				new object(),
				new object()
			};

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			recordExpander.Expect(x => x.Expand(Arg<object>.Is.Equal(records[0]))).Return(expandedRecords).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[1]))).Repeat.Once();

			var expandedDataDestination = new ExpandedDataDestination(recordExpander, dataDestination);

			var returnedDataWriter = expandedDataDestination.CreateDataWriter(dataSource);

			returnedDataWriter.WriteRecord(records[0]);

			dataDestination.VerifyAllExpectations();
			recordExpander.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();
		}

		[TestMethod]
		public void ExpandedDataDestination_MultipleDataDestinationsAndOneExpandedRecord_ExpandedRecordIsWrittenToAllDataDestinations()
		{
			var records = new List<object>()
			{
				new object()
			};
			var expandedRecords = new List<object>()
			{
				new object()
			};

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestinations = new List<IDataDestination>()
			{
				MockRepository.GenerateMock<IDataDestination>(),
				MockRepository.GenerateMock<IDataDestination>()
			};
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();
			var dataWriters = new List<IDataWriter>()
			{
				MockRepository.GenerateMock<IDataWriter>(),
				MockRepository.GenerateMock<IDataWriter>()
			};

			dataDestinations[0].Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriters[0]).Repeat.Once();
			dataDestinations[1].Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriters[1]).Repeat.Once();
			recordExpander.Expect(x => x.Expand(Arg<object>.Is.Equal(records[0]))).Return(expandedRecords).Repeat.Once();
			dataWriters[0].Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();
			dataWriters[1].Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();

			var expandedDataDestination = new ExpandedDataDestination(recordExpander, dataDestinations.ToArray());

			var returnedDataWriter = expandedDataDestination.CreateDataWriter(dataSource);

			returnedDataWriter.WriteRecord(records[0]);

			dataDestinations[0].VerifyAllExpectations();
			dataDestinations[1].VerifyAllExpectations();
			recordExpander.VerifyAllExpectations();
			dataWriters[0].VerifyAllExpectations();
			dataWriters[1].VerifyAllExpectations();
		}

		[TestMethod]
		public void ExpandedDataDestination_MultipleDataDestinationsAndMultipleExpandedRecords_ExpandedRecordsAreWrittenToAllDataDestinations()
		{
			var records = new List<object>()
			{
				new object()
			};
			var expandedRecords = new List<object>()
			{
				new object(),
				new object()
			};

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestinations = new List<IDataDestination>()
			{
				MockRepository.GenerateMock<IDataDestination>(),
				MockRepository.GenerateMock<IDataDestination>()
			};
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();
			var dataWriters = new List<IDataWriter>()
			{
				MockRepository.GenerateMock<IDataWriter>(),
				MockRepository.GenerateMock<IDataWriter>()
			};

			dataDestinations[0].Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriters[0]).Repeat.Once();
			dataDestinations[1].Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriters[1]).Repeat.Once();
			recordExpander.Expect(x => x.Expand(Arg<object>.Is.Equal(records[0]))).Return(expandedRecords).Repeat.Once();
			dataWriters[0].Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();
			dataWriters[0].Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[1]))).Repeat.Once();
			dataWriters[1].Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();
			dataWriters[1].Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[1]))).Repeat.Once();

			var expandedDataDestination = new ExpandedDataDestination(recordExpander, dataDestinations.ToArray());

			var returnedDataWriter = expandedDataDestination.CreateDataWriter(dataSource);

			returnedDataWriter.WriteRecord(records[0]);

			dataDestinations[0].VerifyAllExpectations();
			dataDestinations[1].VerifyAllExpectations();
			recordExpander.VerifyAllExpectations();
			dataWriters[0].VerifyAllExpectations();
			dataWriters[1].VerifyAllExpectations();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void CreateDataWriter_DataDestinationCreateDataWriterThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Anything)).Throw(new InternalTestFailureException());

			var expandedDataDestination = new ExpandedDataDestination(recordExpander, dataDestination);

			expandedDataDestination.CreateDataWriter(dataSource);
		}
	}
}
