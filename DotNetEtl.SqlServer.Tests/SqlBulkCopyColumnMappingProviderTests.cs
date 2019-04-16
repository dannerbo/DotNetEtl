using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class SqlBulkCopyColumnMappingProviderTests
	{
		[TestMethod]
		public void GetColumnMappings_RecordWithDestinationFieldNameAndOrdinalAttributes_ExpectedMappingsAreReturned()
		{
			var sqlBulkCopyColumnMappingProvider = new SqlBulkCopyColumnMappingProvider<TestRecord>();

			var columnMappings = sqlBulkCopyColumnMappingProvider.GetColumnMappings();

			Assert.AreEqual(2, columnMappings.Count());
			Assert.IsNotNull(columnMappings.SingleOrDefault(x => x.DestinationColumn.Equals("FieldWithDestinationFieldName")));
			Assert.IsNotNull(columnMappings.SingleOrDefault(x => x.DestinationOrdinal.Equals(1)));
		}

		private class TestRecord
		{
			[DestinationFieldName("FieldWithDestinationFieldName")]
			public string FieldWithDestinationFieldName { get; set; }

			[DestinationFieldOrdinal(1)]
			public string FieldWithDestinationFieldOrdinal { get; set; }
			
			public string NotUsed { get; set; }
		}
	}
}
