using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordEventArgsTests
	{
		[TestMethod]
		public void Constructor_RecordIsProvided_RecordPropertyIsSet()
		{
			var record = new object();
			var recordEventArgs = new RecordEventArgs(record);

			Assert.AreEqual(record, recordEventArgs.Record);
		}
	}
}
