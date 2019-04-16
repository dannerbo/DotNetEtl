using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordWrittenEventArgsTests
	{
		[TestMethod]
		public void Constructor_RecordAndFormattedRecordAreProvided_PropertiesAreSet()
		{
			var record = new object();
			var formattedRecord = new object();
			var recordWrittenEventArgs = new RecordWrittenEventArgs(record, formattedRecord);

			Assert.AreEqual(record, recordWrittenEventArgs.Record);
			Assert.AreEqual(formattedRecord, recordWrittenEventArgs.FormattedRecord);
		}
	}
}
