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
	public class StreamReaderTests
	{
		[TestMethod]
		public void TryReadRecord_StreamProvidedWithZeroRecords_NoRecordsAreRead()
		{
			var records = String.Empty;
			var readRecord = new Func<System.IO.StreamReader, object>(sr => sr.ReadLine());

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new MockStreamReader(stream, readRecord))
			{
				dataReader.Open();

				var couldReadRecord = dataReader.TryReadRecord(out var record, out var failures);

				Assert.IsFalse(couldReadRecord);
			}
		}

		[TestMethod]
		public void TryReadRecord_StreamProvidedWithOneRecord_RecordsIsRead()
		{
			var recordsRead = 0;
			var records = "Line 1";
			var readRecord = new Func<System.IO.StreamReader, object>(sr => sr.ReadLine());

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new MockStreamReader(stream, readRecord))
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
			var recordsRead = 0;
			var records = "Line 1\nLine 2";
			var readRecord = new Func<System.IO.StreamReader, object>(sr => sr.ReadLine());

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new MockStreamReader(stream, readRecord))
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
		public void TryReadRecord_StreamFactoryProvidedWithOneRecord_RecordsIsRead()
		{
			var recordsRead = 0;
			var records = "Line 1";
			var readRecord = new Func<System.IO.StreamReader, object>(sr => sr.ReadLine());

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			{
				var streamFactory = MockRepository.GenerateMock<IStreamFactory>();

				streamFactory.Expect(x => x.Create()).Return(stream).Repeat.Once();

				using (var dataReader = new MockStreamReader(streamFactory, readRecord))
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

				streamFactory.VerifyAllExpectations();
			}

			Assert.AreEqual(1, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_RecordMapperProvidedWithOneRecord_RecordsIsRead()
		{
			var recordsRead = 0;
			var records = "Line 1";
			var mappedRecords = "Mapped Line 1";
			var readRecord = new Func<System.IO.StreamReader, object>(sr => sr.ReadLine());
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
			using (var dataReader = new MockStreamReader(stream, readRecord, recordMapper))
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
			var records = "Line 1";
			var failures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecord = new Func<System.IO.StreamReader, object>(sr => sr.ReadLine());
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(records.Split('\n')[0]),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(failures).Dummy))
				.Return(false);

			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(records)))
			using (var dataReader = new MockStreamReader(stream, readRecord, recordMapper))
			{
				dataReader.Open();
				
				var couldReadRecord = dataReader.TryReadRecord(out var returnedRecord, out var returnedFailures);

				Assert.IsFalse(couldReadRecord);
				Assert.IsNull(returnedRecord);
				Assert.AreEqual(failures, returnedFailures);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryReadRecord_ReadRecordInternalThrowsException_ExceptionIsPropogated()
		{
			var records = new byte[0];
			var readRecord = new Func<System.IO.StreamReader, object>(sr => throw new InternalTestFailureException());

			using (var stream = new MemoryStream(records))
			using (var dataReader = new MockStreamReader(stream, readRecord))
			{
				dataReader.Open();
				dataReader.TryReadRecord(out var record, out var failures);
			}
		}

		[TestMethod]
		public void Dispose_Dispose_CloseIsCalled()
		{
			var closeWasCalled = false;
			var close = new Action(() => closeWasCalled = true);
			var dataReader = new MockStreamReader(close);

			dataReader.Dispose();

			Assert.IsTrue(closeWasCalled);
		}

		private class MockStreamReader : StreamReader
		{
			private Func<System.IO.StreamReader, object> readRecord;
			private Action close;

			public MockStreamReader(Stream stream, Func<System.IO.StreamReader, object> readRecord, IRecordMapper recordMapper = null)
				: base(stream, recordMapper)
			{
				this.readRecord = readRecord;
			}

			public MockStreamReader(IStreamFactory streamFactory, Func<System.IO.StreamReader, object> readRecord, IRecordMapper recordMapper = null)
				: base(streamFactory, recordMapper)
			{
				this.readRecord = readRecord;
			}

			public MockStreamReader(Action close)
				: base((Stream)null)
			{
				this.close = close;
			}

			public override void Close()
			{
				this.close?.Invoke();

				base.Close();
			}

			protected override object ReadRecordInternal()
			{
				return this.readRecord(this.InternalStreamReader);
			}
		}
	}
}
