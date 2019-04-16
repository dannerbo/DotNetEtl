using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class EnumerableReaderTests
	{
		[TestMethod]
		public void TryReadRecord_EmptyEnumerable_ZeroRecordsAreRead()
		{
			var records = new List<object>()
			{
			};

			var enumerableReader = new EnumerableReader(records);
			
			var couldReadRecord = enumerableReader.TryReadRecord(out var record, out var failures);
			
			Assert.IsFalse(couldReadRecord);
		}

		[TestMethod]
		public void TryReadRecord_OneRecordInEnumerable_OneRecordIsRead()
		{
			var recordsRead = 0;
			var records = new List<object>()
			{
				new object()
			};

			var enumerableReader = new EnumerableReader(records);

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			do
			{
				couldReadRecord = enumerableReader.TryReadRecord(out var record, out failures);

				if (couldReadRecord)
				{
					Assert.AreEqual(records[recordsRead++], record);
					Assert.IsNull(failures);
				}
			}
			while (couldReadRecord || failures?.Count() > 0);

			Assert.AreEqual(records.Count, recordsRead);
		}
		
		[TestMethod]
		public void TryReadRecord_MultipleRecordsInEnumerable_AllRecordsAreRead()
		{
			var recordsRead = 0;
			var records = new List<object>()
			{
				new object(),
				new object()
			};

			var enumerableReader = new EnumerableReader(records);

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			do
			{
				couldReadRecord = enumerableReader.TryReadRecord(out var record, out failures);

				if (couldReadRecord)
				{
					Assert.AreEqual(records[recordsRead++], record);
					Assert.IsNull(failures);
				}
			}
			while (couldReadRecord || failures?.Count() > 0);

			Assert.AreEqual(records.Count, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_OneRecordInEnumerableWithRecordMapper_OneRecordIsMappedAndRead()
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

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records[0]),
					out Arg<object>.Out(mappedRecords[0]).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();

			var enumerableReader = new EnumerableReader(records, recordMapper);

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			do
			{
				couldReadRecord = enumerableReader.TryReadRecord(out var record, out failures);

				if (couldReadRecord)
				{
					Assert.AreEqual(mappedRecords[recordsRead++], record);
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
			var recordIsMappedEventFired = false;
			var records = new List<object>()
			{
				new object()
			};
			var failures = new List<FieldFailure>()
			{
				new FieldFailure()
			};

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(records[0]),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(failures).Dummy))
				.Return(false);

			var enumerableReader = new EnumerableReader(records, recordMapper);

			enumerableReader.RecordMapped += (sender, e) =>
			{
				recordIsMappedEventFired = true;

				Assert.AreEqual(records[0], e.Record);
				Assert.IsFalse(e.WasSuccessful);
			};

			var couldReadRecord = enumerableReader.TryReadRecord(out var returnedRecord, out var returnedFailures);

			Assert.IsFalse(couldReadRecord);
			Assert.IsNull(returnedRecord);
			Assert.AreEqual(failures, returnedFailures);
			Assert.IsTrue(recordIsMappedEventFired);
		}
	}
}
