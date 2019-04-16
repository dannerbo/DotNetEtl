using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DestinationFieldNameAttributeTests
	{
		[TestMethod]
		public void Constructor_FieldNameIsProvided_FieldNamePropertyIsSet()
		{
			var fieldName = "Test";
			var destinationFieldNameAttribute = new DestinationFieldNameAttribute(fieldName);

			Assert.AreEqual(fieldName, destinationFieldNameAttribute.FieldName);
		}
	}
}
