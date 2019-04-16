using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceRecordFieldCountAttributeTests
	{
		[TestMethod]
		public void Constructor_CountIsProvided_PropertyIsSet()
		{
			var count = 10;
			var sourceRecordFieldCountAttribute = new SourceRecordFieldCountAttribute(count);

			Assert.AreEqual(count, sourceRecordFieldCountAttribute.Count);
		}
	}
}
