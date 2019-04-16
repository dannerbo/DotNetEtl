using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DestinationFieldLayoutProviderTests
	{
		[TestMethod]
		public void TryGetDestinationFieldLayout_StringFieldWithAttribute_DestinationFieldLayoutIsReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var destinationFieldLayoutProvider = new DestinationFieldLayoutProvider();

			var couldGetDestinationFieldLayout = destinationFieldLayoutProvider.TryGetDestinationFieldLayout(
				property,
				record,
				out var startIndex,
				out var length);

			Assert.IsTrue(couldGetDestinationFieldLayout);
			Assert.AreEqual(0, startIndex);
			Assert.AreEqual(10, length);
		}

		[TestMethod]
		public void TryGetDestinationFieldLayout_IntFieldWithAttribute_DestinationFieldLayoutIsReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var destinationFieldLayoutProvider = new DestinationFieldLayoutProvider();

			var couldGetDestinationFieldLayout = destinationFieldLayoutProvider.TryGetDestinationFieldLayout(
				property,
				record,
				out var startIndex,
				out var length);

			Assert.IsTrue(couldGetDestinationFieldLayout);
			Assert.AreEqual(10, startIndex);
			Assert.AreEqual(4, length);
		}

		[TestMethod]
		public void TryGetDestinationFieldLayout_FieldWithoutAttribute_DestinationFieldLayoutIsNotReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NotUsed));
			var destinationFieldLayoutProvider = new DestinationFieldLayoutProvider();

			var couldGetDestinationFieldLayout = destinationFieldLayoutProvider.TryGetDestinationFieldLayout(
				property,
				record,
				out var startIndex,
				out var length);

			Assert.IsFalse(couldGetDestinationFieldLayout);
		}

		private class MockRecord
		{
			[DestinationFieldLayout(0, 10)]
			public string StringField { get; set; }
			[DestinationFieldLayout(10, 4)]
			public int IntField { get; set; }
			public string NotUsed { get; set; }
		}
	}
}
