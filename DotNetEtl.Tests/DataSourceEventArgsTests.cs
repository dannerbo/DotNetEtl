using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataSourceEventArgsTests
	{
		[TestMethod]
		public void Constructor_DataSourceIsProvided_DataSourcePropertyIsSet()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataSourceEventArgs = new DataSourceEventArgs(dataSource);

			Assert.AreEqual(dataSource, dataSourceEventArgs.DataSource);
		}
	}
}
