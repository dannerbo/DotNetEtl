using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportEventArgsTests
	{
		[TestMethod]
		public void Constructor_AllArgumentsAreProvided_PropertiesAreSet()
		{
			var dataImport = MockRepository.GenerateMock<IDataImport>();
			
			var dataImportEventArgs = new DataImportEventArgs(dataImport);

			Assert.AreEqual(dataImport, dataImportEventArgs.DataImport);
		}
	}
}
