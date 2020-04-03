using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileStreamFactoryTests
	{
		private const string TestFilesDirectory = "TestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(FileStreamFactoryTests)}_{FileStreamFactoryTests.TestContext.TestName}.txt";

				return Path.Combine(FileStreamFactoryTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			FileStreamFactoryTests.TestContext = testContext;

			if (!Directory.Exists(FileStreamFactoryTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(FileStreamFactoryTests.TestFilesDirectory);
			}

			FileStreamFactoryTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(FileStreamFactoryTests.TestFilesDirectory, $"{nameof(FileStreamFactoryTests)}_*"))
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
		public void Constructor_AllParametersPassed_AllPropertiesAreSet()
		{
			var filePath = "FilePath";
			var fileMode = FileMode.Open;
			var fileAccess = FileAccess.Read;
			var fileShare = FileShare.Read;

			var factory = new FileStreamFactory(filePath, fileMode, fileAccess, fileShare);

			Assert.AreEqual(filePath, factory.FilePath);
			Assert.AreEqual(fileMode, factory.FileMode);
			Assert.AreEqual(fileAccess, factory.FileAccess);
			Assert.AreEqual(fileShare, factory.FileShare);
		}

		[TestMethod]
		public void FilePath_SetFilePathProperty_FilePathPropertyIsSet()
		{
			var filePath = "FilePath";

			var factory = new FileStreamFactory();

			factory.FilePath = filePath;

			Assert.AreEqual(filePath, factory.FilePath);
		}

		[TestMethod]
		public void FileMode_SetFileModeProperty_FileModePropertyIsSet()
		{
			var fileMode = FileMode.Open;

			var factory = new FileStreamFactory();

			factory.FileMode = fileMode;

			Assert.AreEqual(fileMode, factory.FileMode);
		}

		[TestMethod]
		public void FileAccess_SetFileAccessProperty_FileAccessPropertyIsSet()
		{
			var fileAccess = FileAccess.Read;

			var factory = new FileStreamFactory();

			factory.FileAccess = fileAccess;

			Assert.AreEqual(fileAccess, factory.FileAccess);
		}

		[TestMethod]
		public void FileShare_SetFileShareProperty_FileSharePropertyIsSet()
		{
			var fileShare = FileShare.Read;

			var factory = new FileStreamFactory();

			factory.FileShare = fileShare;

			Assert.AreEqual(fileShare, factory.FileShare);
		}

		[TestMethod]
		public void Create_OpenExistingFile_StreamIsReturned()
		{
			var filePath = this.FilePath;
			var fileMode = FileMode.Open;
			var fileAccess = FileAccess.Read;
			var fileShare = FileShare.Read;

			var factory = new FileStreamFactory(filePath, fileMode, fileAccess, fileShare);

			using (var file = File.Create(filePath))
			{
			}

			using (var stream = factory.Create())
			{
				Assert.IsInstanceOfType(stream, typeof(FileStream));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Create_FilePathIsNotSet_ExceptionIsThrown()
		{
			var fileMode = FileMode.Open;
			var fileAccess = FileAccess.Read;
			var fileShare = FileShare.Read;

			var factory = new FileStreamFactory(null, fileMode, fileAccess, fileShare);

			factory.Create();
		}
	}
}
