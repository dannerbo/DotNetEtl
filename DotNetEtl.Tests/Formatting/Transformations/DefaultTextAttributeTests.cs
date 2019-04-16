using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class DefaultTextAttributeTests
	{
		[TestMethod]
		public void Transform_NullValue_DefaultValueReturned()
		{
			var value = (string)null;
			var defaultValue = "Default";
			var defaultTextAttribute = new DefaultTextAttribute(defaultValue);
			var transformedValue = defaultTextAttribute.Transform(value);

			Assert.AreEqual(defaultValue, transformedValue);
		}

		[TestMethod]
		public void Transform_NonNullValue_OriginalValueReturned()
		{
			var value = "NonNull";
			var defaultValue = "Default";
			var defaultTextAttribute = new DefaultTextAttribute(defaultValue);
			var transformedValue = defaultTextAttribute.Transform(value);

			Assert.AreEqual(value, transformedValue);
		}

		[TestMethod]
		public void Transform_NonNullValueWithNullDefaultValue_OriginalValueReturned()
		{
			var value = "NonNull";
			var defaultValue = (string)null;
			var defaultTextAttribute = new DefaultTextAttribute(defaultValue);
			var transformedValue = defaultTextAttribute.Transform(value);

			Assert.AreEqual(value, transformedValue);
		}

		[TestMethod]
		public void Transform_NullValueWithNullDefaultValue_NullValueReturned()
		{
			var value = (string)null;
			var defaultValue = (string)null;
			var defaultTextAttribute = new DefaultTextAttribute(defaultValue);
			var transformedValue = defaultTextAttribute.Transform(value);

			Assert.AreEqual(null, transformedValue);
		}
	}
}
