using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordFormattedEventArgsTests
	{
		[TestMethod]
		public void Constructor_RecordAndFormattedRecordAreProvided_PropertiesAreSet()
		{
			var record = new object();
			var formattedRecord = new object();
			var recordFormattedEventArgs = new RecordFormattedEventArgs(record, formattedRecord);

			Assert.AreEqual(record, recordFormattedEventArgs.Record);
			Assert.AreEqual(formattedRecord, recordFormattedEventArgs.FormattedRecord);
		}
	}
}
