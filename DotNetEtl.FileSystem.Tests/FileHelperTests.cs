using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileHelperTests
	{
		private const string TestFilesDirectory = "TestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(FileHelperTests)}_{FileHelperTests.TestContext.TestName}.txt";

				return Path.Combine(FileHelperTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			FileHelperTests.TestContext = testContext;

			if (!Directory.Exists(FileHelperTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(FileHelperTests.TestFilesDirectory);
			}

			FileHelperTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(FileHelperTests.TestFilesDirectory, $"{nameof(FileHelperTests)}_*"))
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
		[ExpectedException(typeof(FileNotFoundException))]
		public void IsFileLocked_FileDoesNotExist_ExceptionIsThrown()
		{
			FileHelper.IsFileLocked(this.FilePath);
		}

		[TestMethod]
		public void IsFileLocked_NotLockedFile_FalseIsReturned()
		{
			File.WriteAllText(this.FilePath, String.Empty);

			var isFileLocked = FileHelper.IsFileLocked(this.FilePath);

			Assert.IsFalse(isFileLocked);
		}

		[TestMethod]
		public void IsFileLocked_LockedFile_TrueIsReturned()
		{
			using (var file = File.OpenWrite(this.FilePath))
			{
				var isFileLocked = FileHelper.IsFileLocked(this.FilePath);

				Assert.IsTrue(isFileLocked);
			}
		}
	}
}
