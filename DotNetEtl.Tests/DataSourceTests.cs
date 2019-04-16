using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataSourceTests
	{
		[TestMethod]
		public void CreateDataReader_DataReaderIsProvided_ProvidedDataReaderIsReturned()
		{
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataSource = new DataSource(dataReader);
			var returnedDataReader = dataSource.CreateDataReader();

			Assert.AreEqual(dataReader, returnedDataReader);
		}
		
		[TestMethod]
		public void CreateDataReader_DataReaderFactoryIsProvided_DataReaderFromFactoryIsReturned()
		{
			var dataReaderFactory = MockRepository.GenerateMock<IDataReaderFactory>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataSource = new DataSource(dataReaderFactory);

			dataReaderFactory.Expect(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataReader).Repeat.Once();

			var returnedDataReader = dataSource.CreateDataReader();
			
			dataReaderFactory.VerifyAllExpectations();

			Assert.AreEqual(dataReader, returnedDataReader);
		}
		
		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void CreateDataReader_DataReaderFactoryThrowsException_ExceptionIsPropogated()
		{
			var dataReaderFactory = MockRepository.GenerateMock<IDataReaderFactory>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataSource = new DataSource(dataReaderFactory);

			dataReaderFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Anything)).Throw(new InternalTestFailureException());

			dataSource.CreateDataReader();
		}
	}
}
