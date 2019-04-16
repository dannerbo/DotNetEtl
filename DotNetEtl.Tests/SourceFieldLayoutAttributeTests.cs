using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceFieldLayoutAttributeTests
	{
		[TestMethod]
		public void Constructor_StartIndexAndLengthAreProvided_PropertiesAreSet()
		{
			var startIndex = 1;
			var length = 10;
			var sourceFieldLayoutAttribute = new SourceFieldLayoutAttribute(startIndex, length);

			Assert.AreEqual(startIndex, sourceFieldLayoutAttribute.StartIndex);
			Assert.AreEqual(length, sourceFieldLayoutAttribute.Length);
		}
	}
}
