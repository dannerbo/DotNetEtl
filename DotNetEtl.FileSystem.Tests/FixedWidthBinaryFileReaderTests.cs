using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FixedWidthBinaryFileReaderTests
	{
		private const string TestFilesDirectory = "TestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(FixedWidthBinaryFileReaderTests)}_{FixedWidthBinaryFileReaderTests.TestContext.TestName}.txt";

				return Path.Combine(FixedWidthBinaryFileReaderTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			FixedWidthBinaryFileReaderTests.TestContext = testContext;

			if (!Directory.Exists(FixedWidthBinaryFileReaderTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(FixedWidthBinaryFileReaderTests.TestFilesDirectory);
			}

			FileWriterTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(FixedWidthBinaryFileReaderTests.TestFilesDirectory, $"{nameof(FixedWidthBinaryFileReaderTests)}_*"))
			{
				File.Delete(file);
			}
		}

		[TestCleanup]
		public void TestCleanup()
		{
			if (File.Exists(this.FilePath))
			{
				File.Delete(this.FilePath);
			}
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecordsWithHeaderAndFooter_RecordsAreRead()
		{
			var headerBytes = new byte[4];
			var footerBytes = new byte[2];
			var record1Bytes = BitConverter.GetBytes((int)100).Concat("Record 1".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var record2Bytes = BitConverter.GetBytes((int)200).Concat("Record 2".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var fileBytes = headerBytes.Concat(record1Bytes).Concat(record2Bytes).Concat(footerBytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldReadRecord2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldReadRecord3 = fileReader.TryReadRecord(out var record3, out var failures3);

				Assert.IsTrue(couldReadRecord1);
				Assert.IsTrue(couldReadRecord2);
				Assert.IsFalse(couldReadRecord3);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record1, 100, "Record 1");
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record2, 200, "Record 2");
			}
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecordsWithHeader_RecordsAreRead()
		{
			var headerBytes = new byte[4];
			var record1Bytes = BitConverter.GetBytes((int)100).Concat("Record 1".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var record2Bytes = BitConverter.GetBytes((int)200).Concat("Record 2".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var fileBytes = headerBytes.Concat(record1Bytes).Concat(record2Bytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 0;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldReadRecord2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldReadRecord3 = fileReader.TryReadRecord(out var record3, out var failures3);

				Assert.IsTrue(couldReadRecord1);
				Assert.IsTrue(couldReadRecord2);
				Assert.IsFalse(couldReadRecord3);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record1, 100, "Record 1");
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record2, 200, "Record 2");
			}
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecordsWithFooter_RecordsAreRead()
		{
			var footerBytes = new byte[2];
			var record1Bytes = BitConverter.GetBytes((int)100).Concat("Record 1".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var record2Bytes = BitConverter.GetBytes((int)200).Concat("Record 2".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var fileBytes = record1Bytes.Concat(record2Bytes).Concat(footerBytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 0;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldReadRecord2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldReadRecord3 = fileReader.TryReadRecord(out var record3, out var failures3);

				Assert.IsTrue(couldReadRecord1);
				Assert.IsTrue(couldReadRecord2);
				Assert.IsFalse(couldReadRecord3);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record1, 100, "Record 1");
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record2, 200, "Record 2");
			}
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecords_RecordsAreRead()
		{
			var record1Bytes = BitConverter.GetBytes((int)100).Concat("Record 1".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var record2Bytes = BitConverter.GetBytes((int)200).Concat("Record 2".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var fileBytes = record1Bytes.Concat(record2Bytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 0;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 0;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldReadRecord2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldReadRecord3 = fileReader.TryReadRecord(out var record3, out var failures3);

				Assert.IsTrue(couldReadRecord1);
				Assert.IsTrue(couldReadRecord2);
				Assert.IsFalse(couldReadRecord3);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record1, 100, "Record 1");
				FixedWidthBinaryFileReaderTests.AssertRecordMatches((byte[])record2, 200, "Record 2");
			}
		}

		[TestMethod]
		public void TryReadRecord_ZeroRecordsWithHeaderAndFooter_ZeroRecordsAreRead()
		{
			var headerBytes = new byte[4];
			var footerBytes = new byte[2];
			var fileBytes = headerBytes.Concat(footerBytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);

				Assert.IsFalse(couldReadRecord1);
			}
		}

		[TestMethod]
		public void TryReadRecord_ZeroRecords_ZeroRecordsAreRead()
		{
			var fileBytes = new byte[0];

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 0;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 0;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);

				Assert.IsFalse(couldReadRecord1);
			}
		}

		[TestMethod]
		public void TryReadRecord_ZeroRecordsWithHeader_ZeroRecordsAreRead()
		{
			var headerBytes = new byte[4];
			var fileBytes = headerBytes;

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 0;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);

				Assert.IsFalse(couldReadRecord1);
			}
		}

		[TestMethod]
		public void TryReadRecord_ZeroRecordsWithFooter_ZeroRecordsAreRead()
		{
			var footerBytes = new byte[2];
			var fileBytes = footerBytes;

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 0;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);

				Assert.IsFalse(couldReadRecord1);
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_MissingFooter_ExceptionIsThrown()
		{
			var headerBytes = new byte[4];
			var footerBytes = new byte[2];
			var record1Bytes = BitConverter.GetBytes((int)100).Concat("Record 1".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var record2Bytes = BitConverter.GetBytes((int)200).Concat("Record 2".Select(x => (byte)x).ToArray());
			var fileBytes = headerBytes.Concat(record1Bytes).Concat(record2Bytes).Concat(footerBytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				fileReader.TryReadRecord(out var record1, out var failures1);
				fileReader.TryReadRecord(out var record2, out var failures2);
				fileReader.TryReadRecord(out var record3, out var failures3);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_ExtraBytes_ExceptionIsThrown()
		{
			var headerBytes = new byte[4];
			var footerBytes = new byte[2];
			var record1Bytes = BitConverter.GetBytes((int)100).Concat("Record 1".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0 });
			var record2Bytes = BitConverter.GetBytes((int)200).Concat("Record 2".Select(x => (byte)x).ToArray()).Concat(new byte[] { 0, 0, 0, 0 });
			var fileBytes = headerBytes.Concat(record1Bytes).Concat(record2Bytes).Concat(footerBytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				fileReader.TryReadRecord(out var record1, out var failures1);
				fileReader.TryReadRecord(out var record2, out var failures2);
				fileReader.TryReadRecord(out var record3, out var failures3);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_InvalidHeader_ExceptionIsThrown()
		{
			var fileBytes = new byte[] { 0, 0 };

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				fileReader.TryReadRecord(out var record1, out var failures1);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryReadRecord_InvalidRecord_ExceptionIsThrown()
		{
			var headerBytes = new byte[4];
			var footerBytes = new byte[2];
			var record1Bytes = BitConverter.GetBytes((int)100).Concat("Rec".Select(x => (byte)x).ToArray());
			var fileBytes = headerBytes.Concat(record1Bytes).Concat(footerBytes).ToArray();

			File.WriteAllBytes(this.FilePath, fileBytes);

			using (var fileReader = new FixedWidthBinaryFileReader(this.FilePath))
			{
				fileReader.HeaderSize = 4;
				fileReader.RecordSize = 14;
				fileReader.FooterSize = 2;

				fileReader.Open();

				fileReader.TryReadRecord(out var record1, out var failures1);
				fileReader.TryReadRecord(out var record2, out var failures2);
			}
		}

		private static void AssertRecordMatches(byte[] record, int field1, string field2)
		{
			using (var memoryStream = new MemoryStream(record))
			using (var binaryReader = new BinaryReader(memoryStream))
			{
				var field1Actual = binaryReader.ReadInt32();
				var field2Actual = new string(binaryReader.ReadChars(10));

				var nullTerminatorPosition = field2Actual.IndexOf('\0');

				if (nullTerminatorPosition > -1)
				{
					field2Actual = field2Actual.Remove(nullTerminatorPosition);
				}

				Assert.AreEqual(field1, field1Actual);
				Assert.AreEqual(field2, field2Actual);
			}
		}
	}
}
