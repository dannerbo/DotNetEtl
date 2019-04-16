using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class CreateDataImportErrorEventArgsTests
	{
		[TestMethod]
		public void Constructor_ExceptionAndDataSourceProvided_PropertiesAreSet()
		{
			var exception = new InternalTestFailureException();
			var dataSource = MockRepository.GenerateMock<IDataSource>();

			var createDataImportErrorEventArgs = new CreateDataImportErrorEventArgs(exception, dataSource);

			Assert.AreEqual(exception, createDataImportErrorEventArgs.Exception);
			Assert.AreEqual(dataSource, createDataImportErrorEventArgs.DataSource);
		}
	}
}
