using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataReaderTests
	{
		[TestMethod]
		public void TryReadRecord_ZeroRecords_NoRecordsAreRead()
		{
			var readRecord = new Func<object>(() =>
			{
				return null;
			});

			var dataReader = new MockDataReader(readRecord);

			var couldReadRecord = dataReader.TryReadRecord(out var record, out var failures);

			Assert.IsFalse(couldReadRecord);
			Assert.IsNull(failures);
		}

		[TestMethod]
		public void TryReadRecord_OneRecord_RecordIsRead()
		{
			var recordsRead = 0;
			var records = new List<object>()
			{
				new object()
			};

			var readRecord = new Func<object>(() =>
			{
				if (recordsRead == 1)
				{
					return null;
				}

				return records[recordsRead++];
			});

			var dataReader = new MockDataReader(readRecord);

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			do
			{
				couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

				if (couldReadRecord)
				{
					Assert.AreEqual(records[recordsRead - 1], record);
					Assert.IsNull(failures);
				}
			}
			while (couldReadRecord || failures?.Count() > 0);

			Assert.AreEqual(records.Count, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecords_AllRecordsAreRead()
		{
			var recordsRead = 0;
			var records = new List<object>()
			{
				new object(),
				new object(),
				new object()
			};

			var readRecord = new Func<object>(() =>
			{
				if (recordsRead == records.Count)
				{
					return null;
				}

				return records[recordsRead++];
			});

			var dataReader = new MockDataReader(readRecord);

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			do
			{
				couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

				if (couldReadRecord)
				{
					Assert.AreEqual(records[recordsRead - 1], record);
					Assert.IsNull(failures);
				}
			}
			while (couldReadRecord || failures?.Count() > 0);

			Assert.AreEqual(records.Count, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_OneRecordWithRecordMapper_RecordIsMappedAndRead()
		{
			var recordsRead = 0;
			var records = new List<object>()
			{
				new object()
			};
			var mappedRecords = new List<object>()
			{
				new object()
			};

			var readRecord = new Func<object>(() =>
			{
				if (recordsRead == 1)
				{
					return null;
				}

				return records[recordsRead++];
			});

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records[0]),
					out Arg<object>.Out(mappedRecords[0]).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();

			var dataReader = new MockDataReader(recordMapper, readRecord);

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			do
			{
				couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

				if (couldReadRecord)
				{
					Assert.AreEqual(mappedRecords[recordsRead - 1], record);
					Assert.IsNull(failures);
				}
			}
			while (couldReadRecord || failures?.Count() > 0);

			recordMapper.VerifyAllExpectations();

			Assert.AreEqual(records.Count, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_OneRecordThatFailsToMap_RecordIsNotReadAndFailureIsReturned()
		{
			var record = new object();
			var failures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecord = new Func<object>(() => record);

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(failures).Dummy))
				.Return(false);

			var dataReader = new MockDataReader(recordMapper, readRecord);

			var couldReadRecord = dataReader.TryReadRecord(out var returnedRecord, out var returnedFailures);

			Assert.IsFalse(couldReadRecord);
			Assert.IsNull(returnedRecord);
			Assert.AreEqual(failures, returnedFailures);
		}
		
		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryReadRecord_ReadRecordInternalThrowsException_ExceptionIsPropogated()
		{
			var readRecord = new Func<object>(() =>
			{
				throw new InternalTestFailureException();
			});

			var dataReader = new MockDataReader(readRecord);

			dataReader.TryReadRecord(out var record, out var failures);
		}

		[TestMethod]
		public void Dispose_Dispose_CloseIsCalled()
		{
			var closeWasCalled = false;
			var close = new Action(() => closeWasCalled = true);
			var dataReader = new MockDataReader(close);

			dataReader.Dispose();

			Assert.IsTrue(closeWasCalled);
		}

		private class MockDataReader : DataReader
		{
			private Func<object> readRecord;
			private Action close;

			public MockDataReader(IRecordMapper recordMapper, Func<object> readRecord)
				: base(recordMapper)
			{
				this.readRecord = readRecord;
			}

			public MockDataReader(Func<object> readRecord)
				: base()
			{
				this.readRecord = readRecord;
			}

			public MockDataReader(Action close)
			{
				this.close = close;
			}

			public override void Close()
			{
				this.close();

				base.Close();
			}

			protected override object ReadRecordInternal()
			{
				return this.readRecord();
			}
		}
	}
}
