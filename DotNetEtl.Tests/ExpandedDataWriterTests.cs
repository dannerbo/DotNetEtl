using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class ExpandedDataWriterTests
	{
		[TestMethod]
		public void Open_MultipleDataDestinations_AllDataWritersAreOpened()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Expect(x => x.Open());
			dataWriter2.Expect(x => x.Open());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			expandedDataWriter.Open();

			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
		}

		[TestMethod]
		public void Close_MultipleDataDestinations_AllDataWritersAreClosed()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Expect(x => x.Close());
			dataWriter2.Expect(x => x.Close());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			expandedDataWriter.Close();

			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
		}

		[TestMethod]
		public void Commit_MultipleDataDestinations_AllDataWritersAreCommitted()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Expect(x => x.Commit());
			dataWriter2.Expect(x => x.Commit());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			expandedDataWriter.Commit();

			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
		}

		[TestMethod]
		public void Rollback_MultipleDataDestinations_AllDataWritersAreRolledBack()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Expect(x => x.Rollback());
			dataWriter2.Expect(x => x.Rollback());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			expandedDataWriter.Rollback();

			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
		}

		[TestMethod]
		public void Dispose_MultipleDataDestinations_AllDataWritersAreDisposed()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Expect(x => x.Dispose());
			dataWriter2.Expect(x => x.Dispose());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			expandedDataWriter.Dispose();

			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
		}

		[TestMethod]
		public void WriteRecord_MultipleDataWritersAndExpandedRecords_AllDataWritersWriteAllExpandedRecords()
		{
			var record = new object();
			var expandedRecords = new List<object>()
			{
				new object(),
				new object()
			};

			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			recordExpander.Expect(x => x.Expand(Arg<object>.Is.Equal(record))).Return(expandedRecords).Repeat.Once();
			dataWriter1.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();
			dataWriter1.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[1]))).Repeat.Once();
			dataWriter2.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();
			dataWriter2.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[1]))).Repeat.Once();

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			expandedDataWriter.WriteRecord(record);

			recordExpander.VerifyAllExpectations();
			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
		}
		
		[TestMethod]
		public void WriteRecord_MultipleDataWritersAndExpandedRecordsOfDifferentTypeWithRecordFilter_AllDataWritersWriteExpectedFilteredExpandedRecords()
		{
			var record = new object();
			var expandedRecords = new List<object>()
			{
				new object(),
				new object()
			};

			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var recordFilter1 = MockRepository.GenerateMock<IRecordFilter>();
			var recordFilter2 = MockRepository.GenerateMock<IRecordFilter>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataDestination1.Expect(x => x.RecordFilter).Return(recordFilter1);
			dataDestination2.Expect(x => x.RecordFilter).Return(recordFilter2);
			recordFilter1.Expect(x => x.MeetsCriteria(Arg<object>.Is.Equal(expandedRecords[0]))).Return(true).Repeat.Once();
			recordFilter1.Expect(x => x.MeetsCriteria(Arg<object>.Is.Equal(expandedRecords[1]))).Return(false).Repeat.Once();
			recordFilter2.Expect(x => x.MeetsCriteria(Arg<object>.Is.Equal(expandedRecords[0]))).Return(false).Repeat.Once();
			recordFilter2.Expect(x => x.MeetsCriteria(Arg<object>.Is.Equal(expandedRecords[1]))).Return(true).Repeat.Once();
			recordExpander.Expect(x => x.Expand(Arg<object>.Is.Equal(record))).Return(expandedRecords).Repeat.Once();
			dataWriter1.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Once();
			dataWriter1.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[1]))).Repeat.Never();
			dataWriter2.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[0]))).Repeat.Never();
			dataWriter2.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(expandedRecords[1]))).Repeat.Once();

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			expandedDataWriter.WriteRecord(record);

			dataDestination1.VerifyAllExpectations();
			dataDestination2.VerifyAllExpectations();
			recordFilter1.VerifyAllExpectations();
			recordFilter2.VerifyAllExpectations();
			recordExpander.VerifyAllExpectations();
			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
		}

		[TestMethod]
		public void Open_DataWriterOpenThrowsException_ExceptionIsPropogated()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Stub(x => x.Open()).Throw(new InternalTestFailureException());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			try
			{
				expandedDataWriter.Open();
			}
			catch (AggregateException ae)
			{
				Assert.IsTrue(ae.InnerException is InternalTestFailureException);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Close_DataWriterCloseThrowsException_ExceptionIsPropogated()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Stub(x => x.Close()).Throw(new InternalTestFailureException());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			try
			{
				expandedDataWriter.Close();
			}
			catch (AggregateException ae)
			{
				Assert.IsTrue(ae.InnerException is InternalTestFailureException);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Commit_DataWriterCommitThrowsException_ExceptionIsPropogated()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Stub(x => x.Commit()).Throw(new InternalTestFailureException());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			try
			{
				expandedDataWriter.Commit();
			}
			catch (AggregateException ae)
			{
				Assert.IsTrue(ae.InnerException is InternalTestFailureException);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Rollback_DataWriterRollbackThrowsException_ExceptionIsPropogated()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Stub(x => x.Rollback()).Throw(new InternalTestFailureException());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			try
			{
				expandedDataWriter.Rollback();
			}
			catch (AggregateException ae)
			{
				Assert.IsTrue(ae.InnerException is InternalTestFailureException);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Dispose_DataWriterDisposeThrowsException_ExceptionIsPropogated()
		{
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataWriter1.Stub(x => x.Dispose()).Throw(new InternalTestFailureException());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			try
			{
				expandedDataWriter.Dispose();
			}
			catch (AggregateException ae)
			{
				Assert.IsTrue(ae.InnerException is InternalTestFailureException);

				return;
			}

			Assert.Fail();
		}
		
		[TestMethod]
		public void WriteRecord_DataWriterWriteRecordThrowsException_ExceptionIsPropogated()
		{
			var record = new object();
			var expandedRecords = new List<object>()
			{
				new object(),
				new object()
			};

			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			recordExpander.Stub(x => x.Expand(Arg<object>.Is.Equal(record))).Return(expandedRecords);
			dataWriter1.Stub(x => x.WriteRecord(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			try
			{
				expandedDataWriter.WriteRecord(record);
			}
			catch (AggregateException ae)
			{
				ae = ae.Flatten();

				Assert.IsTrue(ae.InnerException is InternalTestFailureException);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void WriteRecord_RecordExpanderExpandThrowsException_ExceptionIsPropogated()
		{
			var record = new object();

			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			recordExpander.Stub(x => x.Expand(Arg<object>.Is.Equal(record))).Throw(new InternalTestFailureException());

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);
			
			expandedDataWriter.WriteRecord(record);
		}

		[TestMethod]
		public void WriteRecord_RecordFilterRecordMeedsCriteriaThrowsException_ExceptionIsPropogated()
		{
			var record = new object();
			var expandedRecords = new List<object>()
			{
				new object(),
				new object()
			};

			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var recordFilter1 = MockRepository.GenerateMock<IRecordFilter>();
			var recordFilter2 = MockRepository.GenerateMock<IRecordFilter>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var recordExpander = MockRepository.GenerateMock<IRecordExpander>();

			dataDestination1.Stub(x => x.RecordFilter).Return(recordFilter1);
			dataDestination2.Stub(x => x.RecordFilter).Return(recordFilter2);
			recordFilter1.Stub(x => x.MeetsCriteria(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());
			recordFilter2.Stub(x => x.MeetsCriteria(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());
			recordExpander.Stub(x => x.Expand(Arg<object>.Is.Equal(record))).Return(expandedRecords);

			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			dataWriters.Add(dataDestination1, dataWriter1);
			dataWriters.Add(dataDestination2, dataWriter2);

			var expandedDataWriter = new ExpandedDataWriter(dataWriters, recordExpander);

			try
			{
				expandedDataWriter.WriteRecord(record);
			}
			catch (AggregateException ae)
			{
				Assert.IsTrue(ae.InnerException is InternalTestFailureException);

				return;
			}

			Assert.Fail();
		}
	}
}
