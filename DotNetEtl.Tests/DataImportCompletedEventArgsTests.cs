using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportCompletedEventArgsTests
	{
		[TestMethod]
		public void Constructor_AllArgumentsAreProvided_PropertiesAreSet()
		{
			var dataImport = MockRepository.GenerateMock<IDataImport>();
			var wasSuccessful = true;
			var failures = MockRepository.GenerateMock<IEnumerable<RecordFailure>>();

			var dataImportCompletedEventArgs = new DataImportCompletedEventArgs(dataImport, wasSuccessful, failures);

			Assert.AreEqual(dataImport, dataImportCompletedEventArgs.DataImport);
			Assert.AreEqual(wasSuccessful, dataImportCompletedEventArgs.WasSuccessful);
			Assert.AreEqual(failures, dataImportCompletedEventArgs.Failures);
		}
	}
}
