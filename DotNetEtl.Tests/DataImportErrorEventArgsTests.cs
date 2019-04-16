using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportErrorEventArgsTests
	{
		[TestMethod]
		public void Constructor_AllArgumentsAreProvided_PropertiesAreSet()
		{
			var exception = new Exception();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			var dataImportErrorEventArgs = new DataImportErrorEventArgs(exception, dataImport);

			Assert.AreEqual(exception, dataImportErrorEventArgs.Exception);
			Assert.AreEqual(dataImport, dataImportErrorEventArgs.DataImport);
		}
	}
}
