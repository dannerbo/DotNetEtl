using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DestinationFieldOrdinalProviderTests
	{
		[TestMethod]
		public void TryGetDestinationFieldOrdinal_StringFieldWithAttribute_DestinationFieldOrdinalIsReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var destinationFieldOrdinalProvider = new DestinationFieldOrdinalProvider();

			var couldGetDestinationFieldOrdinal = destinationFieldOrdinalProvider.TryGetDestinationFieldOrdinal(
				property,
				record,
				out var fieldOrdinal);

			Assert.IsTrue(couldGetDestinationFieldOrdinal);
			Assert.AreEqual(0, fieldOrdinal);
		}

		[TestMethod]
		public void TryGetDestinationFieldOrdinal_IntFieldWithAttribute_DestinationFieldOrdinalIsReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var destinationFieldOrdinalProvider = new DestinationFieldOrdinalProvider();

			var couldGetDestinationFieldOrdinal = destinationFieldOrdinalProvider.TryGetDestinationFieldOrdinal(
				property,
				record,
				out var fieldOrdinal);

			Assert.IsTrue(couldGetDestinationFieldOrdinal);
			Assert.AreEqual(1, fieldOrdinal);
		}

		[TestMethod]
		public void TryGetDestinationFieldOrdinal_FieldWithoutAttribute_DestinationFieldOrdinalIsNotReturned()
		{
			var record = (object)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NotUsed));
			var destinationFieldOrdinalProvider = new DestinationFieldOrdinalProvider();

			var couldGetDestinationFieldOrdinal = destinationFieldOrdinalProvider.TryGetDestinationFieldOrdinal(
				property,
				record,
				out var fieldOrdinal);

			Assert.IsFalse(couldGetDestinationFieldOrdinal);
		}

		private class MockRecord
		{
			[DestinationFieldOrdinal(0)]
			public string StringField { get; set; }
			[DestinationFieldOrdinal(1)]
			public int IntField { get; set; }
			public string NotUsed { get; set; }
		}
	}
}
