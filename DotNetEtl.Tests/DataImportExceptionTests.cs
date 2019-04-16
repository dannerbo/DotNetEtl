using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportExceptionTests
	{
		[TestMethod]
		public void Constructor1_DataImportIsProvided_DataImportPropertyIsSet()
		{
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			var dataImportException = new DataImportException(dataImport);

			Assert.AreEqual(dataImport, dataImportException.DataImport);
		}

		[TestMethod]
		public void Constructor2_DataImportAndMessageAreProvided_DataImportAndMessagePropertiesAreSet()
		{
			var dataImport = MockRepository.GenerateMock<IDataImport>();
			var message = "Test error message";

			var dataImportException = new DataImportException(dataImport, message);

			Assert.AreEqual(dataImport, dataImportException.DataImport);
			Assert.AreEqual(message, dataImportException.Message);
		}

		[TestMethod]
		public void Constructor3_DataImportAndMessageAndInnerExceptionAreProvided_DataImportAndMessageAndInnerExceptionPropertiesAreSet()
		{
			var dataImport = MockRepository.GenerateMock<IDataImport>();
			var message = "Test error message";
			var innerException = new Exception();

			var dataImportException = new DataImportException(dataImport, message, innerException);

			Assert.AreEqual(dataImport, dataImportException.DataImport);
			Assert.AreEqual(message, dataImportException.Message);
			Assert.AreEqual(innerException, dataImportException.InnerException);
		}
	}
}
