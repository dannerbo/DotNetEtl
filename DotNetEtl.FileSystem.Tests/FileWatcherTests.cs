using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileWatcherTests
	{
		private const string TestFilesDirectory = "FileWatcherTestFiles";
		private static TestContext TestContext;

		private string FilePath
		{
			get
			{
				var fileName = $"{nameof(FileWatcherTests)}_{FileWatcherTests.TestContext.TestName}.txt";

				return Path.Combine(FileWatcherTests.TestFilesDirectory, fileName);
			}
		}

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			FileWatcherTests.TestContext = testContext;

			if (!Directory.Exists(FileWatcherTests.TestFilesDirectory))
			{
				Directory.CreateDirectory(FileWatcherTests.TestFilesDirectory);
			}

			FileWatcherTests.Cleanup();
		}

		[ClassCleanup]
		public static void Cleanup()
		{
			foreach (var file in Directory.GetFiles(FileWatcherTests.TestFilesDirectory))
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
		public void FileWatcher_StartAndStop_NoExceptionIsThrown()
		{
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory);

			fileWatcher.Start();
			fileWatcher.Stop();
		}

		[TestMethod]
		public void FileWatcher_FileAvailableAfterStart_FileIsFound()
		{
			var wasFileFound = false;
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory);

			fileWatcher.ShouldIgnoreExistingFiles = false;

			fileDataSourceFactory.Expect(x => x.Create(
					Arg<string>.Is.Equal(this.FilePath)))
				.Return(new FileDataSource(dataReader: null, filePath: this.FilePath))
				.Repeat.Once();

			using (var waitSignal = new ManualResetEventSlim())
			{
				fileWatcher.DataSourceAvailable += (sender, e) =>
				{
					wasFileFound = ((IFileDataSource)e.DataSource).FilePath.Equals(this.FilePath);

					waitSignal.Set();
				};

				fileWatcher.Start();

				File.WriteAllText(this.FilePath, String.Empty);
				
				fileWatcher.FindNewFilesPublic();
				waitSignal.Wait(TimeSpan.FromSeconds(1));
				fileWatcher.Stop();

				fileDataSourceFactory.VerifyAllExpectations();

				Assert.IsTrue(wasFileFound);
			}
		}

		[TestMethod]
		public void FileWatcher_FileAvailableBeforeStartAndShouldIgnoreExistingFilesIsTrue_FileIsNotFound()
		{
			var wasFileFound = false;
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory);

			fileWatcher.ShouldIgnoreExistingFiles = true;

			fileDataSourceFactory.Expect(x => x.Create(Arg<string>.Is.Equal(this.FilePath))).Repeat.Never();

			using (var waitSignal = new ManualResetEventSlim())
			{
				fileWatcher.DataSourceAvailable += (sender, e) =>
				{
					wasFileFound = ((IFileDataSource)e.DataSource).FilePath.Equals(this.FilePath);

					waitSignal.Set();
				};

				File.WriteAllText(this.FilePath, String.Empty);
				
				fileWatcher.Start();
				fileWatcher.FindNewFilesPublic();
				waitSignal.Wait(TimeSpan.FromSeconds(1));
				fileWatcher.Stop();

				fileDataSourceFactory.VerifyAllExpectations();

				Assert.IsFalse(wasFileFound);
			}
		}

		[TestMethod]
		public void FileWatcher_FileAvailableBeforeStartAndShouldIgnoreExistingFilesIsFalse_FileIsFound()
		{
			var wasFileFound = false;
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory);

			fileWatcher.ShouldIgnoreExistingFiles = false;

			fileDataSourceFactory.Expect(x => x.Create(
					Arg<string>.Is.Equal(this.FilePath)))
				.Return(new FileDataSource(dataReader: null, filePath: this.FilePath))
				.Repeat.Once();

			using (var waitSignal = new ManualResetEventSlim())
			{
				fileWatcher.DataSourceAvailable += (sender, e) =>
				{
					wasFileFound = ((IFileDataSource)e.DataSource).FilePath.Equals(this.FilePath);

					waitSignal.Set();
				};

				File.WriteAllText(this.FilePath, String.Empty);
				
				fileWatcher.Start();
				fileWatcher.FindNewFilesPublic();
				waitSignal.Wait(TimeSpan.FromSeconds(1));
				fileWatcher.Stop();

				fileDataSourceFactory.VerifyAllExpectations();

				Assert.IsTrue(wasFileFound);
			}
		}

		[TestMethod]
		public void FileWatcher_LockedFileAvailableAfterStart_FileIsFoundAfterUnlocked()
		{
			var wasFileFound = false;
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory);

			fileWatcher.ShouldIgnoreExistingFiles = false;

			fileDataSourceFactory.Expect(x => x.Create(
					Arg<string>.Is.Equal(this.FilePath)))
				.Return(new FileDataSource(dataReader: null, filePath: this.FilePath))
				.Repeat.Once();

			using (var waitSignal = new ManualResetEventSlim())
			using (var fileUnlockedSignal = new ManualResetEventSlim())
			{
				fileWatcher.DataSourceAvailable += (sender, e) =>
				{
					Assert.IsTrue(fileUnlockedSignal.IsSet);

					wasFileFound = ((IFileDataSource)e.DataSource).FilePath.Equals(this.FilePath);

					waitSignal.Set();
				};

				fileWatcher.Start();
				
				Task.Run(() =>
				{
					using (var file = File.OpenWrite(this.FilePath))
					{
						Thread.Sleep(TimeSpan.FromSeconds(1));
					}

					fileUnlockedSignal.Set();

					fileWatcher.FindNewFilesPublic();
				});
				
				fileWatcher.FindNewFilesPublic();
				waitSignal.Wait(TimeSpan.FromSeconds(2));
				fileWatcher.Stop();

				fileDataSourceFactory.VerifyAllExpectations();

				Assert.IsTrue(wasFileFound);
			}
		}

		[TestMethod]
		public void FileWatcher_FileAvailableAfterStartAndMatchesFilter_FileIsFound()
		{
			var wasFileFound = false;
			var filter = "*.txt";
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory, filter);

			fileWatcher.ShouldIgnoreExistingFiles = false;

			fileDataSourceFactory.Expect(x => x.Create(
					Arg<string>.Is.Equal(this.FilePath)))
				.Return(new FileDataSource(dataReader: null, filePath: this.FilePath))
				.Repeat.Once();

			using (var waitSignal = new ManualResetEventSlim())
			{
				fileWatcher.DataSourceAvailable += (sender, e) =>
				{
					wasFileFound = ((IFileDataSource)e.DataSource).FilePath.Equals(this.FilePath);

					waitSignal.Set();
				};

				fileWatcher.Start();

				File.WriteAllText(this.FilePath, String.Empty);
				
				fileWatcher.FindNewFilesPublic();
				waitSignal.Wait(TimeSpan.FromSeconds(1));
				fileWatcher.Stop();

				fileDataSourceFactory.VerifyAllExpectations();

				Assert.IsTrue(wasFileFound);
			}
		}

		[TestMethod]
		public void FileWatcher_FileAvailableAfterStartAndDoesNotMatchFilter_FileIsNotFound()
		{
			var wasFileFound = false;
			var filter = "*.zzz";
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory, filter);

			fileWatcher.ShouldIgnoreExistingFiles = false;

			using (var waitSignal = new ManualResetEventSlim())
			{
				fileWatcher.DataSourceAvailable += (sender, e) =>
				{
					wasFileFound = ((IFileDataSource)e.DataSource).FilePath.Equals(this.FilePath);

					waitSignal.Set();
				};

				fileWatcher.Start();

				File.WriteAllText(this.FilePath, String.Empty);
				
				fileWatcher.FindNewFilesPublic();
				waitSignal.Wait(TimeSpan.FromSeconds(1));
				fileWatcher.Stop();

				fileDataSourceFactory.VerifyAllExpectations();

				Assert.IsFalse(wasFileFound);
			}
		}

		[TestMethod]
		public void FileWatcher_DataSourceAvailableEventHandlerThrowsException_ErrorEventIsFired()
		{
			var errorEventWasFired = false;
			var exception = new InternalTestFailureException();
			var fileDataSourceFactory = MockRepository.GenerateMock<IFileDataSourceFactory>();
			var fileWatcher = new MockFileWatcher(fileDataSourceFactory, FileWatcherTests.TestFilesDirectory);

			fileWatcher.ShouldIgnoreExistingFiles = false;

			fileDataSourceFactory.Expect(x => x.Create(
					Arg<string>.Is.Equal(this.FilePath)))
				.Return(new FileDataSource(dataReader: null, filePath: this.FilePath))
				.Repeat.Once();

			using (var waitSignal = new ManualResetEventSlim())
			{
				fileWatcher.DataSourceAvailable += (sender, e) => throw exception;
				fileWatcher.Error += (sender, e) =>
				{
					errorEventWasFired = e.Exception.Equals(exception);

					waitSignal.Set();
				};

				fileWatcher.Start();

				File.WriteAllText(this.FilePath, String.Empty);
				
				fileWatcher.FindNewFilesPublic();
				waitSignal.Wait(TimeSpan.FromSeconds(1));
				fileWatcher.Stop();

				fileDataSourceFactory.VerifyAllExpectations();

				Assert.IsTrue(errorEventWasFired);
			}
		}

		private class MockFileWatcher : FileWatcher
		{
			public MockFileWatcher(IFileDataSourceFactory dataSourceFactory, string path, string filter = null)
				: base(dataSourceFactory, path, filter)
			{
			}

			public void FindNewFilesPublic()
			{
				this.FindNewFiles();
			}
		}
	}
}
