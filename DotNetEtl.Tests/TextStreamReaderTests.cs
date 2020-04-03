using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class TextStreamReaderTests
	{
		[TestMethod]
		public void TryReadRecord_StreamProvidedWithZeroRecords_NoRecordsAreRead()
		{
			var records = String.Empty;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new TextStreamReader(stream))
			{
				dataReader.Open();

				var couldReadRecord = dataReader.TryReadRecord(out var record, out var failures);

				Assert.IsFalse(couldReadRecord);
			}
		}

		[TestMethod]
		public void TryReadRecord_StreamProvidedWithOneRecord_RecordIsRead()
		{
			var records = "Record 1";
			var recordsRead = 0;

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new TextStreamReader(stream))
			{
				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.AreEqual(records.Split('\n')[++recordsRead - 1], record);
					}
				}
				while (couldReadRecord || failures?.Count() > 0);
			}

			Assert.AreEqual(1, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_StreamProvidedWithMultipleRecords_AllRecordsAreRead()
		{
			var records = "Record 1\nRecord 2";
			var recordsRead = 0;

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new TextStreamReader(stream))
			{
				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.AreEqual(records.Split('\n')[++recordsRead - 1], record);
					}
				}
				while (couldReadRecord || failures?.Count() > 0);
			}

			Assert.AreEqual(2, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_StreamFactoryProvidedWithOneRecord_RecordIsRead()
		{
			var records = "Record 1";
			var recordsRead = 0;

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			{
				var streamFactory = MockRepository.GenerateMock<IStreamFactory>();

				streamFactory.Expect(x => x.Create()).Return(stream).Repeat.Once();

				using (var dataReader = new TextStreamReader(streamFactory))
				{
					dataReader.Open();

					do
					{
						couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

						if (couldReadRecord)
						{
							Assert.AreEqual(records.Split('\n')[++recordsRead - 1], record);
						}
					}
					while (couldReadRecord || failures?.Count() > 0);
				}
			}

			Assert.AreEqual(1, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_RecordMapperProvidedWithOneRecord_RecordsIsReadAndMapped()
		{
			var records = "Record 1";
			var mappedRecords = "Mapped Record 1";
			var recordsRead = 0;

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records.Split('\n')[0]),
					out Arg<object>.Out(mappedRecords.Split('\n')[0]).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new TextStreamReader(stream, recordMapper))
			{
				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.AreEqual(mappedRecords.Split('\n')[++recordsRead - 1], record);
					}
				}
				while (couldReadRecord || failures?.Count() > 0);
			}

			recordMapper.VerifyAllExpectations();

			Assert.AreEqual(1, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_RecordFailsToMap_RecordIsNotReadAndFailureIsReturned()
		{
			var records = "Record 1";
			var failures = new List<FieldFailure>()
			{
				new FieldFailure()
			};

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(records.Split('\n')[0]),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(failures).Dummy))
				.Return(false);

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new TextStreamReader(stream, recordMapper))
			{
				dataReader.Open();

				var couldReadRecord = dataReader.TryReadRecord(out var returnedRecord, out var returnedFailures);

				Assert.IsFalse(couldReadRecord);
				Assert.IsNull(returnedRecord);
				Assert.AreEqual(failures, returnedFailures);
			}
		}
	}
}
