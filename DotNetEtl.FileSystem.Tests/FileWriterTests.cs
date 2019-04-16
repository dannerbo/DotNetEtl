using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileWriterTests
	{
		private const string TestFilesDirectory = "TestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(FileWriterTests)}_{FileWriterTests.TestContext.TestName}.txt";

				return Path.Combine(FileWriterTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			FileWriterTests.TestContext = testContext;

			if (!Directory.Exists(FileWriterTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(FileWriterTests.TestFilesDirectory);
			}

			FileWriterTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(FileWriterTests.TestFilesDirectory, $"{nameof(FileWriterTests)}_*"))
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
		public void Open_ValidFilePath_FileIsOpened()
		{
			using (var fileWriter = new MockFileWriter(this.FilePath))
			{
				fileWriter.Open();

				Assert.IsTrue(fileWriter.IsFileOpen);
			}
		}

		[TestMethod]
		public void Dispose_AfterFileOpened_FileIsClosed()
		{
			var fileWriter = new MockFileWriter(this.FilePath);

			fileWriter.Open();
			fileWriter.Dispose();

			Assert.IsTrue(fileWriter.IsFileClosed);
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
			
			using (var fileWriter = new MockFileWriter(this.FilePath))
			{
				fileWriter.Open();

				FileWriterTests.WriteRecords(fileWriter, records);
			}

			FileWriterTests.AssertFileMatches(this.FilePath, records);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void WriteRecord_WriteRecordInternalThrowsException_ExceptionIsPropogated()
		{
			var record = "TestRecord";

			using (var fileWriter = new MockErroringFileWriter())
			{
				fileWriter.WriteRecord(record);
			}
		}

		[TestMethod]
		public void WriteRecord_MultipleRecordsWithRecordFormatter_RecordsAreFormattedAndWritten()
		{
			var records = new string[]
			{
				"Line 1",
				"Line 2",
				"Line 3"
			};

			var formattedRecords = new string[]
			{
				"Formatted Line 1",
				"Formatted Line 2",
				"Formatted Line 3"
			};

			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(records[0]))).Return(formattedRecords[0]).Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(records[1]))).Return(formattedRecords[1]).Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(records[2]))).Return(formattedRecords[2]).Repeat.Once();

			using (var fileWriter = new MockFileWriter(this.FilePath, recordFormatter))
			{
				fileWriter.Open();

				FileWriterTests.WriteRecords(fileWriter, records);
			}

			recordFormatter.VerifyAllExpectations();

			FileWriterTests.AssertFileMatches(this.FilePath, formattedRecords);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void WriteRecord_RecordFormatterThrowsException_ExceptionIsPropogated()
		{
			var records = new string[]
			{
				"Line 1",
				"Line 2",
				"Line 3"
			};

			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			using (var fileWriter = new MockFileWriter(this.FilePath, recordFormatter))
			{
				fileWriter.Open();

				FileWriterTests.WriteRecords(fileWriter, records);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(IOException))]
		public void FileShare_None_FileIsNotShared()
		{
			using (var fileWriter = new MockFileWriter(this.FilePath))
			{
				fileWriter.Open();

				File.OpenRead(this.FilePath);
			}
		}

		[TestMethod]
		public void FileShare_ReadWrite_FileIsShared()
		{
			using (var fileWriter = new MockFileWriter(this.FilePath, fileShare: FileShare.ReadWrite))
			{
				fileWriter.Open();

				using (var file = File.Open(this.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
				}
			}
		}

		private static void WriteRecords(MockFileWriter fileWriter, string[] records)
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

		private class MockFileWriter : FileWriter
		{
			private StreamWriter streamWriter;

			public MockFileWriter(string filePath, FileMode fileMode = FileMode.CreateNew, FileShare fileShare = FileShare.None)
				: base(filePath, fileMode, fileShare)
			{
			}

			public MockFileWriter(string filePath, IRecordFormatter recordFormatter, FileMode fileMode = FileMode.CreateNew, FileShare fileShare = FileShare.None)
				: base(filePath, recordFormatter, fileMode, fileShare)
			{
			}

			public bool IsFileOpen => (bool)this.FileStream?.CanWrite;
			public bool IsFileClosed => this.FileStream == null || !this.FileStream.CanWrite;

			public override void Open()
			{
				base.Open();

				this.streamWriter = new StreamWriter(this.FileStream);
			}

			public override void Close()
			{
				if (this.streamWriter != null)
				{
					this.streamWriter.Dispose();
					this.streamWriter = null;
				}

				base.Close();
			}

			protected override void WriteRecordInternal(object record)
			{
				this.streamWriter.WriteLine(record);
			}
		}
		
		private class MockErroringFileWriter : FileWriter
		{
			public MockErroringFileWriter()
				: base(null)
			{
			}

			public override void Open()
			{
			}

			public override void Close()
			{
			}

			protected override void WriteRecordInternal(object record)
			{
				throw new InternalTestFailureException();
			}
		}
	}
}
