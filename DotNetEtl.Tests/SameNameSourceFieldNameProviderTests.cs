using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SameNameSourceFieldNameProviderTests
	{
		[TestMethod]
		public void TryGetSourceFieldName_RecordIsProvided_FieldNameReturnedIsSameAsPropertyName()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var record = new MockRecord();
			var sameNameSourceFieldNameProvider = new SameNameSourceFieldNameProvider();

			var couldGetSourceFieldName = sameNameSourceFieldNameProvider.TryGetSourceFieldName(property, record, out var fieldName);

			Assert.IsTrue(couldGetSourceFieldName);
			Assert.AreEqual(nameof(MockRecord.StringField), fieldName);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
		}
	}
}
