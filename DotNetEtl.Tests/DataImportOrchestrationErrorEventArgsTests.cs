using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportOrchestrationErrorEventArgsTests
	{
		[TestMethod]
		public void Constructor1_ExceptionIsProvided_ExceptionPropertyIsSet()
		{
			var exception = new Exception();

			var dataImportOrchestrationErrorEventArgs = new DataImportOrchestrationErrorEventArgs(exception);

			Assert.AreEqual(exception, dataImportOrchestrationErrorEventArgs.Exception);
		}

		[TestMethod]
		public void Constructor3_AllArgumentsAreProvided_PropertiesAreSet()
		{
			var exception = new Exception();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			var dataImportOrchestrationErrorEventArgs = new DataImportOrchestrationErrorEventArgs(exception, dataSource, dataImport);

			Assert.AreEqual(exception, dataImportOrchestrationErrorEventArgs.Exception);
			Assert.AreEqual(dataSource, dataImportOrchestrationErrorEventArgs.DataSource);
			Assert.AreEqual(dataImport, dataImportOrchestrationErrorEventArgs.DataImport);
		}
	}
}
