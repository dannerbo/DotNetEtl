using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceFieldNameProviderTests
	{
		[TestMethod]
		public void TryGetSourceFieldName_RecordWithAttributeIsProvided_FieldNameIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithAttribute));
			var record = new MockRecord();
			var sourceFieldNameProvider = new SourceFieldNameProvider();

			var couldGetSourceFieldName = sourceFieldNameProvider.TryGetSourceFieldName(property, record, out var fieldName);

			Assert.IsTrue(couldGetSourceFieldName);
			Assert.AreEqual("TestField", fieldName);
		}

		[TestMethod]
		public void TryGetSourceFieldName_RecordWithoutAttributeIsProvided_FieldNameIsNotReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutAttribute));
			var record = new MockRecord();
			var sourceFieldNameProvider = new SourceFieldNameProvider();

			var couldGetSourceFieldName = sourceFieldNameProvider.TryGetSourceFieldName(property, record, out var fieldName);

			Assert.IsFalse(couldGetSourceFieldName);
		}

		private class MockRecord
		{
			[SourceFieldName("TestField")]
			public string FieldWithAttribute { get; set; }
			public string FieldWithoutAttribute { get; set; }
		}
	}
}
