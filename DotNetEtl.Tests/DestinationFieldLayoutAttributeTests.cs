using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DestinationFieldLayoutAttributeTests
	{
		[TestMethod]
		public void Constructor_ProvideStartIndexAndLength_StartIndexAndLengthPropertiesAreSet()
		{
			var startIndex = 1;
			var length = 2;

			var destinationFieldLayoutAttribute = new DestinationFieldLayoutAttribute(startIndex, length);

			Assert.AreEqual(startIndex, destinationFieldLayoutAttribute.StartIndex);
			Assert.AreEqual(length, destinationFieldLayoutAttribute.Length);
		}
	}
}
