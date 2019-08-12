using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordEventArgsTests
	{
		[TestMethod]
		public void Constructor_RecordIsProvided_RecordPropertyIsSet()
		{
			var recordIndex = 1;
			var record = new object();
			var recordEventArgs = new RecordEventArgs(recordIndex, record);

			Assert.AreEqual(recordIndex, recordEventArgs.RecordIndex);
			Assert.AreEqual(record, recordEventArgs.Record);
		}
	}
}
