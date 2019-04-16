using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceRecordFieldCountProviderTests
	{
		[TestMethod]
		public void TryGetSourceRecordFieldCount_RecordWithAttributeIsProvided_FieldCountIsReturned()
		{
			var record = new MockRecordWithAttribute();
			var sourceRecordFieldCountprovider = new SourceRecordFieldCountProvider();

			var couldGetSourceRecordFieldCount = sourceRecordFieldCountprovider.TryGetSourceRecordFieldCount(
				typeof(MockRecordWithAttribute),
				record,
				out var count);

			Assert.IsTrue(couldGetSourceRecordFieldCount);
			Assert.AreEqual(10, count);
		}

		[TestMethod]
		public void TryGetSourceRecordFieldCount_RecordWithoutAttributeIsProvided_FieldCountIsNotReturned()
		{
			var record = new MockRecordWithoutAttribute();
			var sourceRecordFieldCountprovider = new SourceRecordFieldCountProvider();

			var couldGetSourceRecordFieldCount = sourceRecordFieldCountprovider.TryGetSourceRecordFieldCount(
				typeof(MockRecordWithoutAttribute),
				record,
				out var count);

			Assert.IsFalse(couldGetSourceRecordFieldCount);
		}

		[SourceRecordFieldCount(10)]
		private class MockRecordWithAttribute
		{
		}
		
		private class MockRecordWithoutAttribute
		{
		}
	}
}
