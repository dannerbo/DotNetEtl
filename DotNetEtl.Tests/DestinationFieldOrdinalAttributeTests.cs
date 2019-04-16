using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DestinationFieldOrdinalAttributeTests
	{
		[TestMethod]
		public void Constructor_FieldOrdinalIsProvided_FieldOrdinalPropertyIsSet()
		{
			var ordinal = 1;
			var destinationFieldOrdinalAttribute = new DestinationFieldOrdinalAttribute(ordinal);

			Assert.AreEqual(ordinal, destinationFieldOrdinalAttribute.Ordinal);
		}
	}
}
