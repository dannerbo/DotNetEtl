using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordFilterTests
	{
		[TestMethod]
		public void MeetsCriteria_TypesMatch_DoesNotMeetCriteria()
		{
			var record = String.Empty;
			var recordFilter = new RecordFilter<string>();

			var meetsCriteria = recordFilter.MeetsCriteria(record);

			Assert.IsTrue(meetsCriteria);
		}

		[TestMethod]
		public void MeetsCriteria_TypesDoNotMatch_DoesNotMeetCriteria()
		{
			var record = 0;
			var recordFilter = new RecordFilter<string>();

			var meetsCriteria = recordFilter.MeetsCriteria(record);

			Assert.IsFalse(meetsCriteria);
		}
	}
}
