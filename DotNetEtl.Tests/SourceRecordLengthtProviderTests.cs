using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceRecordLengthProviderTests
	{
		[TestMethod]
		public void TryGetSourceRecordLength_RecordWithAttributeIsProvided_LengthIsReturned()
		{
			var record = new MockRecordWithAttribute();
			var sourceRecordLengthprovider = new SourceRecordLengthProvider();

			var couldGetSourceRecordLength = sourceRecordLengthprovider.TryGetSourceRecordLength(
				typeof(MockRecordWithAttribute),
				record,
				out var length);

			Assert.IsTrue(couldGetSourceRecordLength);
			Assert.AreEqual(10, length);
		}

		[TestMethod]
		public void TryGetSourceRecordLength_RecordWithoutAttributeIsProvided_LengthIsNotReturned()
		{
			var record = new MockRecordWithoutAttribute();
			var sourceRecordLengthprovider = new SourceRecordLengthProvider();

			var couldGetSourceRecordLength = sourceRecordLengthprovider.TryGetSourceRecordLength(
				typeof(MockRecordWithoutAttribute),
				record,
				out var length);

			Assert.IsFalse(couldGetSourceRecordLength);
		}

		[SourceRecordLength(10)]
		private class MockRecordWithAttribute
		{
		}
		
		private class MockRecordWithoutAttribute
		{
		}
	}
}
