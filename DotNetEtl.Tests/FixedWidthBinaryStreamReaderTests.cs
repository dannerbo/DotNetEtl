using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class FixedWidthBinaryStreamReaderTests
	{
		[TestMethod]
		public void TryReadRecord_NoHeaderOrFooterWithZeroRecords_NoRecordsAreRead()
		{
			var records = new byte[] { };
			
			using (var stream = new MemoryStream(records))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 0;
				dataReader.FooterSize = 0;

				dataReader.Open();

				var couldReadRecord = dataReader.TryReadRecord(out var record, out var failures);

				Assert.IsFalse(couldReadRecord);
			}
		}

		[TestMethod]
		public void TryReadRecord_HeaderAndFooterWithZeroRecords_NoRecordsAreRead()
		{
			var records = new byte[] { 0x1, 0x2 };

			using (var stream = new MemoryStream(records))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 1;
				dataReader.FooterSize = 1;

				dataReader.Open();

				var couldReadRecord = dataReader.TryReadRecord(out var record, out var failures);

				Assert.IsFalse(couldReadRecord);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_HeaderAndFooterMissingWithZeroRecords_ExceptionIsThrown()
		{
			var records = new byte[] { };

			using (var stream = new MemoryStream(records))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 1;
				dataReader.FooterSize = 1;

				dataReader.Open();

				dataReader.TryReadRecord(out var record, out var failures);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_HeaderMissingAndNoFooterWithZeroRecords_ExceptionIsThrown()
		{
			var records = new byte[] { };

			using (var stream = new MemoryStream(records))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 1;
				dataReader.FooterSize = 0;

				dataReader.Open();

				dataReader.TryReadRecord(out var record, out var failures);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_FooterMissingAndNoHeaderWithZeroRecords_ExceptionIsThrown()
		{
			var records = new byte[] { };

			using (var stream = new MemoryStream(records))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 0;
				dataReader.FooterSize = 1;

				dataReader.Open();

				dataReader.TryReadRecord(out var record, out var failures);
			}
		}

		[TestMethod]
		public void TryReadRecord_NoHeaderOrFooterWithOneRecord_RecordIsRead()
		{
			var records = new byte[][]
			{
				new byte[] { 0x1, 0x2 }
			};
			var recordsBuffer = BuildRecordsBuffer(records);
			var recordsRead = 0;

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;
			
			using (var stream = new MemoryStream(recordsBuffer))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 0;
				dataReader.FooterSize = 0;

				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.IsTrue(((byte[])record).SequenceEqual(records[++recordsRead - 1]));
					}
				}
				while (couldReadRecord || failures?.Count() > 0);

				Assert.AreEqual(1, recordsRead);
			}
		}

		[TestMethod]
		public void TryReadRecord_NoHeaderOrFooterWithTwoRecords_AllRecordsAreRead()
		{
			var records = new byte[][]
			{
				new byte[] { 0x1, 0x2 },
				new byte[] { 0x3, 0x4 }
			};
			var recordsBuffer = BuildRecordsBuffer(records);
			var recordsRead = 0;

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(recordsBuffer))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 0;
				dataReader.FooterSize = 0;

				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.IsTrue(((byte[])record).SequenceEqual(records[++recordsRead - 1]));
					}
				}
				while (couldReadRecord || failures?.Count() > 0);

				Assert.AreEqual(2, recordsRead);
			}
		}

		[TestMethod]
		public void TryReadRecord_HeaderAndFooterWithOneRecord_RecordIsRead()
		{
			var records = new byte[][]
			{
				new byte[] { 0x1 },
				new byte[] { 0x2, 0x3 },
				new byte[] { 0x4 }
			};
			var recordsBuffer = BuildRecordsBuffer(records);
			var recordsRead = 0;

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(recordsBuffer))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 1;
				dataReader.FooterSize = 1;

				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.IsTrue(((byte[])record).SequenceEqual(records[++recordsRead]));
					}
				}
				while (couldReadRecord || failures?.Count() > 0);

				Assert.AreEqual(1, recordsRead);
			}
		}

		[TestMethod]
		public void TryReadRecord_HeaderAndFooterWithTwoRecords_AllRecordsAreRead()
		{
			var records = new byte[][]
			{
				new byte[] { 0x1 },
				new byte[] { 0x2, 0x3 },
				new byte[] { 0x4, 0x5 },
				new byte[] { 0x6 }
			};
			var recordsBuffer = BuildRecordsBuffer(records);
			var recordsRead = 0;

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(recordsBuffer))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 1;
				dataReader.FooterSize = 1;

				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);

					if (couldReadRecord)
					{
						Assert.IsTrue(((byte[])record).SequenceEqual(records[++recordsRead]));
					}
				}
				while (couldReadRecord || failures?.Count() > 0);

				Assert.AreEqual(2, recordsRead);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_HeaderAndFooterWithInvalidRecordLength_ExceptionIsThrown()
		{
			var records = new byte[][]
			{
				new byte[] { 0x1 },
				new byte[] { 0x2, 0x3 },
				new byte[] { 0x4, 0x5, 0x6 },
				new byte[] { 0x7 }
			};
			var recordsBuffer = BuildRecordsBuffer(records);

			bool couldReadRecord;
			IEnumerable<FieldFailure> failures;

			using (var stream = new MemoryStream(recordsBuffer))
			using (var dataReader = new FixedWidthBinaryStreamReader(stream))
			{
				dataReader.RecordSize = 2;
				dataReader.HeaderSize = 1;
				dataReader.FooterSize = 1;

				dataReader.Open();

				do
				{
					couldReadRecord = dataReader.TryReadRecord(out var record, out failures);
				}
				while (couldReadRecord || failures?.Count() > 0);
			}
		}

		private byte[] BuildRecordsBuffer(byte[][] records)
		{
			var bufferLength = 0;

			for (int i = 0; i < records.Length; i++)
			{
				bufferLength += records[i].Length;
			}

			var buffer = new byte[bufferLength];
			var position = 0;

			for (int i = 0; i < records.Length; i++)
			{
				Buffer.BlockCopy(records[i], 0, buffer, position, records[i].Length);

				position += records[i].Length;
			}
			
			return buffer;
		}
	}
}
