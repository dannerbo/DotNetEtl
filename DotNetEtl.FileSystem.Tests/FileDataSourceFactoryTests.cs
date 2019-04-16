using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileDataSourceFactoryTests
	{
		[TestMethod]
		public void Create_CreateDataSourceFuncIsProvided_FileDataSourceIsCreated()
		{
			var filePath = @"C:\Temp\testfile.txt";
			var fileDataSource = MockRepository.GenerateMock<IFileDataSource>();
			var createDataSourceFunc = new Func<string, IFileDataSource>(fp =>
				{
					Assert.AreEqual(filePath, fp);

					return fileDataSource;
				});
			var fileDataSourceFactory = new FileDataSourceFactory(createDataSourceFunc);

			var fileDataSourceReturned = fileDataSourceFactory.Create(filePath);

			Assert.AreEqual(fileDataSource, fileDataSourceReturned);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_CreateDataSourceFuncThrowsException_ExceptionIsPropogated()
		{
			var filePath = @"C:\Temp\testfile.txt";
			var fileDataSource = MockRepository.GenerateMock<IFileDataSource>();
			var createDataSourceFunc = new Func<string, IFileDataSource>(fp => throw new InternalTestFailureException());
			var fileDataSourceFactory = new FileDataSourceFactory(createDataSourceFunc);

			fileDataSourceFactory.Create(filePath);
		}

		[TestMethod]
		public void Create_DataReaderFactoryIsProvided_FileDataSourceIsCreated()
		{
			var filePath = @"C:\Temp\testfile.txt";
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataReaderFactory = MockRepository.GenerateMock<IDataReaderFactory>();
			var fileDataSourceFactory = new FileDataSourceFactory<MockFileDataSource>(dataReaderFactory);

			var fileDataSourceReturned = fileDataSourceFactory.Create(filePath);

			Assert.IsInstanceOfType(fileDataSourceReturned, typeof(MockFileDataSource));
			Assert.AreEqual(filePath, fileDataSourceReturned.FilePath);
		}

		[TestMethod]
		public void Create_CreateDataSourceFuncIsProvidedWithGenericImplementation_FileDataSourceIsCreated()
		{
			var filePath = @"C:\Temp\testfile.txt";
			var dataReaderFactory = MockRepository.GenerateMock<IDataReaderFactory>();
			var fileDataSource = new MockFileDataSource(dataReaderFactory, filePath);
			var createDataSourceFunc = new Func<string, MockFileDataSource>(fp =>
			{
				Assert.AreEqual(filePath, fp);

				return fileDataSource;
			});
			var fileDataSourceFactory = new FileDataSourceFactory<MockFileDataSource>(createDataSourceFunc);

			var fileDataSourceReturned = fileDataSourceFactory.Create(filePath);

			Assert.AreEqual(fileDataSource, fileDataSourceReturned);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_CreateDataSourceFuncThrowsExceptionWithGenericImplementation_ExceptionIsPropogated()
		{
			var filePath = @"C:\Temp\testfile.txt";
			var dataReaderFactory = MockRepository.GenerateMock<IDataReaderFactory>();
			var fileDataSource = new MockFileDataSource(dataReaderFactory, filePath);
			var createDataSourceFunc = new Func<string, MockFileDataSource>(fp => throw new InternalTestFailureException());
			var fileDataSourceFactory = new FileDataSourceFactory<MockFileDataSource>(createDataSourceFunc);

			fileDataSourceFactory.Create(filePath);
		}

		private class MockFileDataSource : IFileDataSource
		{
			private IDataReaderFactory dataReaderFactory;

			public MockFileDataSource(IDataReaderFactory dataReaderFactory, string filePath)
			{
				this.dataReaderFactory = dataReaderFactory;
				this.FilePath = filePath;
			}

			public string FilePath { get; private set; }

			public IDataReader CreateDataReader()
			{
				return dataReaderFactory.Create(this);
			}
		}
	}
}
