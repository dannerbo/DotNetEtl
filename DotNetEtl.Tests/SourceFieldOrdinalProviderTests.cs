using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceFieldOrdinalProviderTests
	{
		[TestMethod]
		public void TryGetSourceFieldOrdinal_RecordWithAttributeIsProvided_FieldOrdinalIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithAttribute));
			var record = new MockRecord();
			var sourceFieldOrdinalProvider = new SourceFieldOrdinalProvider();

			var couldGetSourceFieldOrdinal = sourceFieldOrdinalProvider.TryGetSourceFieldOrdinal(property, record, out var ordinal);

			Assert.IsTrue(couldGetSourceFieldOrdinal);
			Assert.AreEqual(1, ordinal);
		}

		[TestMethod]
		public void TryGetSourceFieldOrdinal_RecordWithoutAttributeIsProvided_FieldOrdinalIsNotReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutAttribute));
			var record = new MockRecord();
			var sourceFieldOrdinalProvider = new SourceFieldOrdinalProvider();

			var couldGetSourceFieldOrdinal = sourceFieldOrdinalProvider.TryGetSourceFieldOrdinal(property, record, out var ordinal);

			Assert.IsFalse(couldGetSourceFieldOrdinal);
		}

		private class MockRecord
		{
			[SourceFieldOrdinal(1)]
			public string FieldWithAttribute { get; set; }
			public string FieldWithoutAttribute { get; set; }
		}
	}
}
