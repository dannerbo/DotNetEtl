using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class ToLowerAttributeTests
	{
		[TestMethod]
		public void Transform_UpperCaseText_LowerCaseTextReturned()
		{
			var value = "TEST";
			var toLowerAttribute = new ToLowerAttribute();
			var transformedValue = toLowerAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_LowerCaseText_LowerCaseTextReturned()
		{
			var value = "test";
			var toLowerAttribute = new ToLowerAttribute();
			var transformedValue = toLowerAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_EmptyString_EmptyStringReturned()
		{
			var value = String.Empty;
			var toLowerAttribute = new ToLowerAttribute();
			var transformedValue = toLowerAttribute.Transform(value);

			Assert.AreEqual(String.Empty, transformedValue);
		}
		
		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void Transform_NullValue_ExceptionIsThrown()
		{
			var toLowerAttribute = new ToLowerAttribute();
			var transformedValue = toLowerAttribute.Transform(null);
		}
	}
}
