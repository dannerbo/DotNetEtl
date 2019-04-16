using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordEvaluatedEventArgsTests
	{
		[TestMethod]
		public void Constructor_RecordAndWasSuccessfulAndFailuresAreProvided_AllPropertiesAreSet()
		{
			var record = new object();
			var wasSuccessful = true;
			var failures = new FieldFailure[0];

			var recordEvaluatedEventArgs = new RecordEvaluatedEventArgs(record, wasSuccessful, failures);

			Assert.AreEqual(record, recordEvaluatedEventArgs.Record);
			Assert.AreEqual(wasSuccessful, recordEvaluatedEventArgs.WasSuccessful);
			Assert.AreEqual(failures, recordEvaluatedEventArgs.Failures);
		}
	}
}
