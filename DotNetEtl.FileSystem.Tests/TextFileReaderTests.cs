using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class TextFileReaderTests
	{
		private const string TestFilesDirectory = "TestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(TextFileReaderTests)}_{TextFileReaderTests.TestContext.TestName}.txt";

				return Path.Combine(TextFileReaderTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TextFileReaderTests.TestContext = testContext;

			if (!Directory.Exists(TextFileReaderTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(TextFileReaderTests.TestFilesDirectory);
			}

			TextFileReaderTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(TextFileReaderTests.TestFilesDirectory, $"{nameof(TextFileReaderTests)}_*"))
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
		public void TryReadRecord_MultipleRecords_RecordsAreRead()
		{
			var records = new string[]
			{
				"1,A",
				"2,B"
			};

			File.WriteAllLines(this.FilePath, records);

			using (var fileReader = new TextFileReader(this.FilePath))
			{
				fileReader.HeaderRowCount = 0;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldReadRecord2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldReadRecord3 = fileReader.TryReadRecord(out var record3, out var failures3);

				Assert.IsTrue(couldReadRecord1);
				Assert.IsTrue(couldReadRecord2);
				Assert.IsFalse(couldReadRecord3);
				Assert.AreEqual(records[0], record1);
				Assert.AreEqual(records[1], record2);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
			}
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecordsWithOneHeaderRow_RecordsAreRead()
		{
			var records = new string[]
			{
				"Header",
				"1,A",
				"2,B"
			};

			File.WriteAllLines(this.FilePath, records);

			using (var fileReader = new TextFileReader(this.FilePath))
			{
				fileReader.HeaderRowCount = 1;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldReadRecord2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldReadRecord3 = fileReader.TryReadRecord(out var record3, out var failures3);

				Assert.IsTrue(couldReadRecord1);
				Assert.IsTrue(couldReadRecord2);
				Assert.IsFalse(couldReadRecord3);
				Assert.AreEqual(records[1], record1);
				Assert.AreEqual(records[2], record2);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
			}
		}
		
		[TestMethod]
		public void TryReadRecord_MultipleRecordsWithTwoHeaderRows_RecordsAreRead()
		{
			var records = new string[]
			{
				"Header1",
				"Header2",
				"1,A",
				"2,B"
			};

			File.WriteAllLines(this.FilePath, records);

			using (var fileReader = new TextFileReader(this.FilePath))
			{
				fileReader.HeaderRowCount = 2;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldReadRecord2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldReadRecord3 = fileReader.TryReadRecord(out var record3, out var failures3);

				Assert.IsTrue(couldReadRecord1);
				Assert.IsTrue(couldReadRecord2);
				Assert.IsFalse(couldReadRecord3);
				Assert.AreEqual(records[2], record1);
				Assert.AreEqual(records[3], record2);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
			}
		}

		[TestMethod]
		public void TryReadRecord_ZeroRecords_ZeroRecordsAreRead()
		{
			var records = new string[]
			{
			};

			File.WriteAllLines(this.FilePath, records);

			using (var fileReader = new TextFileReader(this.FilePath))
			{
				fileReader.HeaderRowCount = 0;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);

				Assert.IsFalse(couldReadRecord1);
			}
		}

		[TestMethod]
		public void TryReadRecord_ZeroRecordsWithOneHeaderRow_ZeroRecordsAreRead()
		{
			var records = new string[]
			{
				"Header"
			};

			File.WriteAllLines(this.FilePath, records);

			using (var fileReader = new TextFileReader(this.FilePath))
			{
				fileReader.HeaderRowCount = 1;

				fileReader.Open();

				var couldReadRecord1 = fileReader.TryReadRecord(out var record1, out var failures1);

				Assert.IsFalse(couldReadRecord1);
			}
		}
	}
}
