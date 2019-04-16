using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceFieldOrdinalAttributeTests
	{
		[TestMethod]
		public void Constructor_FieldOrdinalIsProvided_PropertyIsSet()
		{
			var ordinal = 1;
			var sourceFieldOrdinalAttribute = new SourceFieldOrdinalAttribute(ordinal);

			Assert.AreEqual(ordinal, sourceFieldOrdinalAttribute.Ordinal);
		}
	}
}
