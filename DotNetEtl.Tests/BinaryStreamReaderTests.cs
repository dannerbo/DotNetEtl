using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class BinaryStreamReaderTests
	{
		[TestMethod]
		public void TryReadRecord_StreamProvidedWithZeroRecords_NoRecordsAreRead()
		{
			var records = new byte[0];

			var readRecord = new Func<BinaryReader, object>(br =>
			{
				if (br.BaseStream.Position >= records.Length)
				{
					return null;
				}

				return br.ReadByte();
			});

			using (var stream = new MemoryStream(records))
			using (var dataReader = new MockBinaryStreamReader(stream, readRecord))
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
			var records = new byte[] { 0x1 };

			var readRecord = new Func<BinaryReader, object>(br =>
			{
				if (br.BaseStream.Position >= records.Length)
				{
					return null;
				}

				recordsRead++;

				return br.ReadByte();
			});

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(records))
			using (var dataReader = new MockBinaryStreamReader(stream, readRecord))
			{
				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.AreEqual(records[recordsRead - 1], record);
					}
				}
				while (couldReadRecord || failures?.Count() > 0);
			}

			Assert.AreEqual(records.Length, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_StreamProvidedWithMultipleRecords_AllRecordsAreRead()
		{
			var recordsRead = 0;
			var records = new byte[] { 0x1, 0x2 };

			var readRecord = new Func<BinaryReader, object>(br =>
			{
				if (br.BaseStream.Position >= records.Length)
				{
					return null;
				}

				recordsRead++;

				return br.ReadByte();
			});

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(records))
			using (var dataReader = new MockBinaryStreamReader(stream, readRecord))
			{
				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.AreEqual(records[recordsRead - 1], record);
					}
				}
				while (couldReadRecord || failures?.Count() > 0);
			}

			Assert.AreEqual(records.Length, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_StreamFactoryProvidedWithOneRecord_RecordsIsRead()
		{
			var recordsRead = 0;
			var records = new byte[] { 0x1 };

			var readRecord = new Func<BinaryReader, object>(br =>
			{
				if (br.BaseStream.Position >= records.Length)
				{
					return null;
				}

				recordsRead++;

				return br.ReadByte();
			});

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(records))
			{
				var streamFactory = MockRepository.GenerateMock<IStreamFactory>();

				streamFactory.Expect(x => x.Create()).Return(stream).Repeat.Once();

				using (var dataReader = new MockBinaryStreamReader(streamFactory, readRecord))
				{
					dataReader.Open();

					do
					{
						couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

						if (couldReadRecord)
						{
							Assert.AreEqual(records[recordsRead - 1], record);
						}
					}
					while (couldReadRecord || failures?.Count() > 0);
				}

				streamFactory.VerifyAllExpectations();
			}

			Assert.AreEqual(records.Length, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_RecordMapperProvidedWithOneRecord_RecordsIsReadAndMapped()
		{
			var recordsRead = 0;
			var records = new byte[] { 0x1 };
			var mappedRecords = new byte[] { 0x2 };

			var readRecord = new Func<BinaryReader, object>(br =>
			{
				if (br.BaseStream.Position >= records.Length)
				{
					return null;
				}

				recordsRead++;

				return br.ReadByte();
			});

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records[0]),
					out Arg<object>.Out(mappedRecords[0]).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(records))
			using (var dataReader = new MockBinaryStreamReader(stream, readRecord, recordMapper))
			{
				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.AreEqual(mappedRecords[recordsRead - 1], record);
					}
				}
				while (couldReadRecord || failures?.Count() > 0);
			}

			recordMapper.VerifyAllExpectations();

			Assert.AreEqual(records.Length, recordsRead);
		}

		[TestMethod]
		public void TryReadRecord_RecordFailsToMap_RecordIsNotReadAndFailureIsReturned()
		{
			var recordsRead = 0;
			var records = new byte[] { 0x1 };
			var failures = new List<FieldFailure>()
			{
				new FieldFailure()
			};

			var readRecord = new Func<BinaryReader, object>(br =>
			{
				if (br.BaseStream.Position >= records.Length)
				{
					return null;
				}

				recordsRead++;

				return br.ReadByte();
			});

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(records[0]),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(failures).Dummy))
				.Return(false);

			using (var stream = new MemoryStream(records))
			using (var dataReader = new MockBinaryStreamReader(stream, readRecord, recordMapper))
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
			var readRecord = new Func<BinaryReader, object>(br => throw new InternalTestFailureException());

			using (var stream = new MemoryStream(records))
			using (var dataReader = new MockBinaryStreamReader(stream, readRecord))
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
			var dataReader = new MockBinaryStreamReader(close);

			dataReader.Dispose();

			Assert.IsTrue(closeWasCalled);
		}

		private class MockBinaryStreamReader : BinaryStreamReader
		{
			private Func<BinaryReader, object> readRecord;
			private Action close;

			public MockBinaryStreamReader(Stream stream, Func<BinaryReader, object> readRecord, IRecordMapper recordMapper = null)
				: base(stream, recordMapper)
			{
				this.readRecord = readRecord;
			}

			public MockBinaryStreamReader(IStreamFactory streamFactory, Func<BinaryReader, object> readRecord, IRecordMapper recordMapper = null)
				: base(streamFactory, recordMapper)
			{
				this.readRecord = readRecord;
			}

			public MockBinaryStreamReader(Action close)
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
				return this.readRecord(this.InternalBinaryReader);
			}
		}
	}
}
