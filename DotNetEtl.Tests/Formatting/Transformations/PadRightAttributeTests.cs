using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class PadRightAttributeTests
	{
		[TestMethod]
		public void Transform_FullLengthValue_OriginalValueReturned()
		{
			var value = "11111";
			var totalWidth = 5;
			var paddingChar = '0';
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);
			var transformedValue = padRightAttribute.Transform(value);

			Assert.AreEqual(value, transformedValue);
		}

		[TestMethod]
		public void Transform_UnderTotalLengthValue_PaddedValueReturned()
		{
			var value = "111";
			var totalWidth = 5;
			var paddingChar = '0';
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);
			var transformedValue = padRightAttribute.Transform(value);

			Assert.AreEqual("11100", transformedValue);
		}

		[TestMethod]
		public void Transform_OverTotalLengthValue_OriginalValueReturned()
		{
			var value = "111111";
			var totalWidth = 5;
			var paddingChar = '0';
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);
			var transformedValue = padRightAttribute.Transform(value);

			Assert.AreEqual(value, transformedValue);
		}

		[TestMethod]
		public void Transform_ZeroLengthValue_PaddedValueReturned()
		{
			var value = "";
			var totalWidth = 5;
			var paddingChar = '0';
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);
			var transformedValue = padRightAttribute.Transform(value);

			Assert.AreEqual("00000", transformedValue);
		}

		[TestMethod]
		public void Transform_NullValue_PaddedValueReturned()
		{
			var totalWidth = 5;
			var paddingChar = '0';
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);
			var transformedValue = padRightAttribute.Transform(null);

			Assert.AreEqual("00000", transformedValue);
		}
	}
}
