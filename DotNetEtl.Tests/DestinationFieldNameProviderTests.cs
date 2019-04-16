using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DestinationFieldNameProviderTests
	{
		[TestMethod]
		public void TryGetDestinationFieldName_StringFieldWithAttribute_DestinationFieldNameIsReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var destinationFieldNameProvider = new DestinationFieldNameProvider();

			var couldGetDestinationFieldName = destinationFieldNameProvider.TryGetDestinationFieldName(
				property,
				record,
				out var fieldName);

			Assert.IsTrue(couldGetDestinationFieldName);
			Assert.AreEqual("StringFieldX", fieldName);
		}

		[TestMethod]
		public void TryGetDestinationFieldName_IntFieldWithAttribute_DestinationFieldNameIsReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var destinationFieldNameProvider = new DestinationFieldNameProvider();

			var couldGetDestinationFieldName = destinationFieldNameProvider.TryGetDestinationFieldName(
				property,
				record,
				out var fieldName);

			Assert.IsTrue(couldGetDestinationFieldName);
			Assert.AreEqual("IntFieldX", fieldName);
		}

		[TestMethod]
		public void TryGetDestinationFieldName_FieldWithoutAttribute_DestinationFieldNameIsNotReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NotUsed));
			var destinationFieldNameProvider = new DestinationFieldNameProvider();

			var couldGetDestinationFieldName = destinationFieldNameProvider.TryGetDestinationFieldName(
				property,
				record,
				out var fieldName);

			Assert.IsFalse(couldGetDestinationFieldName);
		}

		private class MockRecord
		{
			[DestinationFieldName("StringFieldX")]
			public string StringField { get; set; }
			[DestinationFieldName("IntFieldX")]
			public int IntField { get; set; }
			public string NotUsed { get; set; }
		}
	}
}
