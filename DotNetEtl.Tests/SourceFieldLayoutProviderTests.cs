using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceFieldLayoutProviderTests
	{
		[TestMethod]
		public void TryGetSourceFieldLayout_RecordWithAttributeIsProvided_FieldLayoutIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithAttribute));
			var record = new MockRecord();
			var sourceFieldLayoutProvider = new SourceFieldLayoutProvider();

			var couldGetSourceFieldLayout = sourceFieldLayoutProvider.TryGetSourceFieldLayout(property, record, out var startIndex, out var length);

			Assert.IsTrue(couldGetSourceFieldLayout);
			Assert.AreEqual(1, startIndex);
			Assert.AreEqual(10, length);
		}

		[TestMethod]
		public void TryGetSourceFieldLayout_RecordWithoutAttributeIsProvided_FieldLayoutIsNotReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutAttribute));
			var record = new MockRecord();
			var sourceFieldLayoutProvider = new SourceFieldLayoutProvider();

			var couldGetSourceFieldLayout = sourceFieldLayoutProvider.TryGetSourceFieldLayout(property, record, out var startIndex, out var length);

			Assert.IsFalse(couldGetSourceFieldLayout);
		}

		private class MockRecord
		{
			[SourceFieldLayout(1, 10)]
			public string FieldWithAttribute { get; set; }
			public string FieldWithoutAttribute { get; set; }
		}
	}
}
