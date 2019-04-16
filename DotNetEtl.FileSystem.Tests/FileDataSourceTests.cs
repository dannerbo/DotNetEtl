using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileDataSourceTests
	{
		[TestMethod]
		public void Constructor_DataReaderAndFilePathIsProvided_FilePathPropertyIsSet()
		{
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var filePath = @"C:\Temp\testfile.txt";
			var fileDataSource = new FileDataSource(dataReader, filePath);
			
			Assert.AreEqual(filePath, fileDataSource.FilePath);
		}

		[TestMethod]
		public void Constructor_DataReaderFactoryAndFilePathIsProvided_FilePathPropertyIsSet()
		{
			var dataReaderFactory = MockRepository.GenerateMock<IDataReaderFactory>();
			var filePath = @"C:\Temp\testfile.txt";
			var fileDataSource = new FileDataSource(dataReaderFactory, filePath);

			Assert.AreEqual(filePath, fileDataSource.FilePath);
		}

		[TestMethod]
		public void CreateDataReader_DataReaderIsProvided_DataReaderReturnedIsSameAsDataReaderPassedInConstructor()
		{
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var filePath = @"C:\Temp\testfile.txt";
			var fileDataSource = new FileDataSource(dataReader, filePath);

			var dataReaderReturned = fileDataSource.CreateDataReader();

			Assert.AreEqual(dataReader, dataReaderReturned);
		}

		[TestMethod]
		public void CreateDataReader_DataReaderFactoryIsProvided_DataReaderReturnedIsFromFactory()
		{
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataReaderFactory = MockRepository.GenerateMock<IDataReaderFactory>();
			var filePath = @"C:\Temp\testfile.txt";
			var fileDataSource = new FileDataSource(dataReaderFactory, filePath);

			dataReaderFactory.Expect(x => x.Create(Arg<IDataSource>.Is.Equal(fileDataSource))).Return(dataReader).Repeat.Once();

			var dataReaderReturned = fileDataSource.CreateDataReader();

			dataReaderFactory.VerifyAllExpectations();

			Assert.AreEqual(dataReader, dataReaderReturned);
		}
	}
}
