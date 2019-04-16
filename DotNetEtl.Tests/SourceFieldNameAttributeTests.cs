using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class SourceFieldNameAttributeTests
	{
		[TestMethod]
		public void Constructor_FieldNameIsProvided_PropertyIsSet()
		{
			var fieldName = "TestField";
			var sourceFieldNameAttribute = new SourceFieldNameAttribute(fieldName);

			Assert.AreEqual(fieldName, sourceFieldNameAttribute.FieldName);
		}
	}
}
