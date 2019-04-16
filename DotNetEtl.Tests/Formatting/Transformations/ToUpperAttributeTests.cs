using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class ToUpperAttributeTests
	{
		[TestMethod]
		public void Transform_LowerCaseText_UpperCaseTextReturned()
		{
			var value = "test";
			var toUpperAttribute = new ToUpperAttribute();
			var transformedValue = toUpperAttribute.Transform(value);

			Assert.AreEqual("TEST", transformedValue);
		}

		[TestMethod]
		public void Transform_UpperCaseText_UpperCaseTextReturned()
		{
			var value = "TEST";
			var toUpperAttribute = new ToUpperAttribute();
			var transformedValue = toUpperAttribute.Transform(value);

			Assert.AreEqual("TEST", transformedValue);
		}

		[TestMethod]
		public void Transform_EmptyString_EmptyStringReturned()
		{
			var value = String.Empty;
			var toUpperAttribute = new ToUpperAttribute();
			var transformedValue = toUpperAttribute.Transform(value);

			Assert.AreEqual(String.Empty, transformedValue);
		}
		
		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void Transform_NullValue_ExceptionIsThrown()
		{
			var toUpperAttribute = new ToUpperAttribute();
			var transformedValue = toUpperAttribute.Transform(null);
		}
	}
}
