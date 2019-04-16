using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class TextFileWriterTests
	{
		private const string TestFilesDirectory = "TestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(TextFileWriterTests)}_{TextFileWriterTests.TestContext.TestName}.txt";

				return Path.Combine(TextFileWriterTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TextFileWriterTests.TestContext = testContext;

			if (!Directory.Exists(TextFileWriterTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(TextFileWriterTests.TestFilesDirectory);
			}

			TextFileWriterTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(TextFileWriterTests.TestFilesDirectory, $"{nameof(TextFileWriterTests)}_*"))
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
		public void WriteRecord_MultipleRecords_RecordsAreWritten()
		{
			var records = new string[]
			{
				"Line 1",
				"Line 2",
				"Line 3"
			};

			using (var fileWriter = new TextFileWriter(this.FilePath))
			{
				fileWriter.Open();

				TextFileWriterTests.WriteRecords(fileWriter, records);
			}

			TextFileWriterTests.AssertFileMatches(this.FilePath, records);
		}

		private static void WriteRecords(TextFileWriter fileWriter, string[] records)
		{
			for (var i = 0; i < records.Length; i++)
			{
				fileWriter.WriteRecord(records[i]);
			}
		}

		private static void AssertFileMatches(string filePath, string[] records)
		{
			var fileRecords = File.ReadAllLines(filePath);

			CollectionAssert.AreEqual(records, fileRecords);
		}
	}
}
