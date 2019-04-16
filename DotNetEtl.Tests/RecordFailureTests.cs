using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordFailureTests
	{
		[TestMethod]
		public void Constructor_EmptyConstructor_PropertiesAreNotSet()
		{
			var recordFailure = new RecordFailure();

			Assert.AreEqual(default(int), recordFailure.RecordIndex);
			Assert.AreEqual(default(IEnumerable<FieldFailure>), recordFailure.FieldFailures);
		}

		[TestMethod]
		public void Constructor_RecordIndexAndFieldFailuresAreProvided_PropertiesAreSet()
		{
			var recordIndex = 1;
			var fieldFailures = new FieldFailure[0];
			var recordFailure = new RecordFailure(recordIndex, fieldFailures);

			Assert.AreEqual(recordIndex, recordFailure.RecordIndex);
			Assert.AreEqual(fieldFailures, recordFailure.FieldFailures);
		}
	}
}
