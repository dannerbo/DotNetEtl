using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataWriterTests
	{
		[TestMethod]
		public void WriteRecord_OneRecord_RecordIsWritten()
		{
			var record = new object();
			var recordWritten = (object)null;
			var writeRecord = new Action<object>(r => recordWritten = r);

			var dataWriter = new MockDataWriter(writeRecord);

			dataWriter.WriteRecord(record);

			Assert.AreEqual(record, recordWritten);
		}

		[TestMethod]
		public void WriteRecord_TwoRecords_RecordsAreWritten()
		{
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordsWritten = new List<object>();
			var writeRecord = new Action<object>(r => recordsWritten.Add(r));

			var dataWriter = new MockDataWriter(writeRecord);

			foreach (var record in records)
			{
				dataWriter.WriteRecord(record);
			}

			CollectionAssert.AreEquivalent(records, recordsWritten);
		}

		[TestMethod]
		public void WriteRecord_OneRecordWithRecordFormatter_RecordIsFormattedAndWrittenAndEventIsFired()
		{
			var recordFormattedEventFired = false;
			var record = new object();
			var formattedRecord = new object();
			var recordWritten = (object)null;
			var writeRecord = new Action<object>(r => recordWritten = r);
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(record))).Return(formattedRecord).Repeat.Once();

			var dataWriter = new MockDataWriter(recordFormatter, writeRecord);

			dataWriter.RecordFormatted += (sender, e) =>
			{
				if (e.Record.Equals(record) && e.FormattedRecord.Equals(formattedRecord))
				{
					recordFormattedEventFired = true;
				}
			};

			dataWriter.WriteRecord(record);

			recordFormatter.VerifyAllExpectations();

			Assert.AreEqual(formattedRecord, recordWritten);
			Assert.IsTrue(recordFormattedEventFired);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void WriteRecord_RecordFormatterThrowsException_ExceptionIsPropogated()
		{
			var record = new object();
			var formattedRecord = new object();
			var writeRecord = new Action<object>(r => Assert.Fail());
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Stub(x => throw new InternalTestFailureException());

			var dataWriter = new MockDataWriter(recordFormatter, writeRecord);

			dataWriter.WriteRecord(record);
		}

		[TestMethod]
		public void Dispose_Dispose_CloseIsCalled()
		{
			var closeWasCalled = false;
			var close = new Action(() => closeWasCalled = true);
			var dataReader = new MockDataWriter(close);

			dataReader.Dispose();

			Assert.IsTrue(closeWasCalled);
		}

		private class MockDataWriter : DataWriter
		{
			private Action<object> writeRecord;
			private Action close;

			public MockDataWriter(IRecordFormatter recordFormatter, Action<object> writeRecord)
				: base(recordFormatter)
			{
				this.writeRecord = writeRecord;
			}

			public MockDataWriter(Action<object> writeRecord)
				: this(null, writeRecord)
			{
			}

			public MockDataWriter(Action close)
			{
				this.close = close;
			}

			public override void Commit()
			{
				throw new NotImplementedException();
			}

			public override void Rollback()
			{
				throw new NotImplementedException();
			}

			public override void Close()
			{
				this.close();

				base.Close();
			}

			protected override void WriteRecordInternal(object record)
			{
				this.writeRecord(record);
			}
		}
	}
}
