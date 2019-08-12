using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordWrittenEventArgsTests
	{
		[TestMethod]
		public void Constructor_RecordAndFormattedRecordAreProvided_PropertiesAreSet()
		{
			var recordIndex = 1;
			var record = new object();
			var formattedRecord = new object();
			var recordWrittenEventArgs = new RecordWrittenEventArgs(recordIndex, record, formattedRecord);

			Assert.AreEqual(recordIndex, recordWrittenEventArgs.RecordIndex);
			Assert.AreEqual(record, recordWrittenEventArgs.Record);
			Assert.AreEqual(formattedRecord, recordWrittenEventArgs.FormattedRecord);
		}
	}
}
