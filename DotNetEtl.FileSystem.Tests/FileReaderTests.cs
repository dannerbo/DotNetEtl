using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileReaderTests
	{
		private const string TestFilesDirectory = "TestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(FileReaderTests)}_{FileReaderTests.TestContext.TestName}.txt";

				return Path.Combine(FileReaderTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			FileReaderTests.TestContext = testContext;

			if (!Directory.Exists(FileReaderTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(FileReaderTests.TestFilesDirectory);
			}

			FileReaderTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(FileReaderTests.TestFilesDirectory, $"{nameof(FileReaderTests)}_*"))
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
		public void Open_ExistingFile_FileIsOpened()
		{
			File.WriteAllText(this.FilePath, String.Empty);
			
			using (var fileReader = new MockFileReader(this.FilePath))
			{
				fileReader.Open();

				Assert.IsTrue(fileReader.IsFileOpen);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void Open_FileDoesNotExist_ExceptionIsThrown()
		{
			var nonExistantFilePath = @"C:\MissingFile.txt";

			using (var fileReader = new MockFileReader(nonExistantFilePath))
			{
				fileReader.Open();
			}
		}

		[TestMethod]
		public void Dispose_AfterFileOpened_FileIsClosed()
		{
			File.WriteAllText(this.FilePath, String.Empty);
			
			var fileReader = new MockFileReader(this.FilePath);

			fileReader.Open();
			fileReader.Dispose();

			Assert.IsTrue(fileReader.IsFileClosed);
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecords_RecordsAreRead()
		{
			var records = new string[]
			{
				"Line 1",
				"Line 2",
				"Line 3"
			};
			
			using (var fileReader = new MockFileReader(records))
			{
				var couldBeRead1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldBeRead2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldBeRead3 = fileReader.TryReadRecord(out var record3, out var failures3);
				var couldBeRead4 = fileReader.TryReadRecord(out var record4, out var failures4);

				Assert.IsTrue(couldBeRead1);
				Assert.IsTrue(couldBeRead2);
				Assert.IsTrue(couldBeRead3);
				Assert.IsFalse(couldBeRead4);
				Assert.AreEqual(records[0], record1);
				Assert.AreEqual(records[1], record2);
				Assert.AreEqual(records[2], record3);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
				Assert.IsNull(failures3);
				Assert.IsNull(failures4);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryReadRecord_ReadRecordInternalThrowsException_ExceptionIsPropogated()
		{
			using (var fileReader = new MockErroringFileReader())
			{
				fileReader.TryReadRecord(out var record, out var failures);
			}
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecordsWithRecordMapper_RecordsAreReadAndMapped()
		{
			var records = new string[]
			{
				"Line 1",
				"Line 2",
				"Line 3"
			};

			var mappedRecords = new string[]
			{
				"Mapped 1",
				"Mapped 2",
				"Mapped 3"
			};

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records[0]),
					out Arg<object>.Out(mappedRecords[0]).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records[1]),
					out Arg<object>.Out(mappedRecords[1]).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records[2]),
					out Arg<object>.Out(mappedRecords[2]).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();

			using (var fileReader = new MockFileReader(records, recordMapper))
			{
				var couldBeRead1 = fileReader.TryReadRecord(out var record1, out var failures1);
				var couldBeRead2 = fileReader.TryReadRecord(out var record2, out var failures2);
				var couldBeRead3 = fileReader.TryReadRecord(out var record3, out var failures3);
				var couldBeRead4 = fileReader.TryReadRecord(out var record4, out var failures4);

				Assert.IsTrue(couldBeRead1);
				Assert.IsTrue(couldBeRead2);
				Assert.IsTrue(couldBeRead3);
				Assert.IsFalse(couldBeRead4);
				Assert.AreEqual(mappedRecords[0], record1);
				Assert.AreEqual(mappedRecords[1], record2);
				Assert.AreEqual(mappedRecords[2], record3);
				Assert.IsNull(failures1);
				Assert.IsNull(failures2);
				Assert.IsNull(failures3);
				Assert.IsNull(failures4);
			}

			recordMapper.VerifyAllExpectations();
		}

		[TestMethod]
		public void TryReadRecord_MappingFailure_RecordNotReadAndFailureIsReturned()
		{
			var records = new string[]
			{
				"Line 1"
			};

			var fieldFailures = new FieldFailure[]
			{
				new FieldFailure()
				{
					FieldName = "TestField",
					Message = "Field is invalid."
				}
			};

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(records[0]),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(fieldFailures).Dummy))
				.Return(false)
				.Repeat.Once();

			using (var fileReader = new MockFileReader(records, recordMapper))
			{
				var couldBeRead = fileReader.TryReadRecord(out var record, out var failures);

				Assert.IsFalse(couldBeRead);
				Assert.AreEqual(fieldFailures[0], failures.Single());
			}

			recordMapper.VerifyAllExpectations();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryReadRecord_RecordMapperThrowsException_ExceptionIsPropogated()
		{
			var records = new string[]
			{
				"Line 1",
				"Line 2",
				"Line 3"
			};
			
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			using (var fileReader = new MockFileReader(records, recordMapper))
			{
				fileReader.TryReadRecord(out var record, out var failures);
			}

			recordMapper.VerifyAllExpectations();
		}

		[TestMethod]
		[ExpectedException(typeof(IOException))]
		public void FileShare_None_FileIsNotShared()
		{
			File.WriteAllText(this.FilePath, String.Empty);
			
			using (var fileReader = new MockFileReader(this.FilePath, fileShare: FileShare.None))
			{
				fileReader.Open();

				File.OpenRead(this.FilePath);
			}
		}

		[TestMethod]
		public void FileShare_ReadWrite_FileIsShared()
		{
			File.WriteAllText(this.FilePath, String.Empty);
			
			using (var fileReader = new MockFileReader(this.FilePath, fileShare: FileShare.ReadWrite))
			{
				fileReader.Open();
					
				using (var file = File.OpenRead(this.FilePath))
				{
				}
			}
		}

		private class MockFileReader : FileReader
		{
			private int recordCounter;
			private string[] records;

			public MockFileReader(string filePath, IRecordMapper recordMapper = null, FileShare fileShare = FileShare.None)
				: base(filePath, recordMapper, fileShare)
			{
			}

			public MockFileReader(string[] records, IRecordMapper recordMapper = null, FileShare fileShare = FileShare.None)
				: base(null, recordMapper, fileShare)
			{
				this.records = records;
			}

			public bool IsFileOpen => (bool)this.FileStream?.CanRead;
			public bool IsFileClosed => this.FileStream == null || !this.FileStream.CanRead;

			public override void Open()
			{
				if (this.FilePath != null)
				{
					base.Open();
				}
			}

			public override void Close()
			{
				if (this.FilePath != null)
				{
					base.Close();
				}
			}

			protected override object ReadRecordInternal()
			{
				if (this.FilePath != null)
				{
					throw new InvalidOperationException();
				}

				if (this.recordCounter < this.records.Length)
				{
					return this.records[this.recordCounter++];
				}
				
				return null;
			}
		}

		private class MockErroringFileReader : FileReader
		{
			public MockErroringFileReader()
				: base(null)
			{
			}

			public override void Open()
			{
			}

			public override void Close()
			{
			}

			protected override object ReadRecordInternal()
			{
				throw new InternalTestFailureException();
			}
		}
	}
}
