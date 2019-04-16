using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceRecordLengthAttributeTests
	{
		[TestMethod]
		public void Constructor_LengthIsProvided_PropertyIsSet()
		{
			var length = 10;
			var sourceRecordLengthAttribute = new SourceRecordLengthAttribute(length);

			Assert.AreEqual(length, sourceRecordLengthAttribute.Length);
		}
	}
}
