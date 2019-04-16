using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class ReplaceTextAttributeTests
	{
		[TestMethod]
		public void Transform_ValueWithTextToReplace_OriginalValueWithTextReplacedReturned()
		{
			var value = "ABC";
			var oldValue = "A";
			var newValue = "B";
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);
			var transformedValue = replaceTextAttribute.Transform(value);

			Assert.AreEqual("BBC", transformedValue);
		}

		[TestMethod]
		public void Transform_ValueWithoutTextToReplace_OriginalValueReturned()
		{
			var value = "ABC";
			var oldValue = "Z";
			var newValue = "B";
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);
			var transformedValue = replaceTextAttribute.Transform(value);

			Assert.AreEqual(value, transformedValue);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void Transform_NullValue_ExceptionIsThrown()
		{
			var oldValue = "A";
			var newValue = "B";
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);
			var transformedValue = replaceTextAttribute.Transform(null);
		}

		[TestMethod]
		public void Transform_ValueWithTextToReplaceAndNewValueIsNull_OriginalValueWithTextReplacedReturned()
		{
			var value = "ABC";
			var oldValue = "A";
			var newValue = (string)null;
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);
			var transformedValue = replaceTextAttribute.Transform(value);

			Assert.AreEqual("BC", transformedValue);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Transform_ValueWithTextToReplaceAndOldValueIsNull_ExceptionIsThrown()
		{
			var value = "ABC";
			var oldValue = (string)null;
			var newValue = "B";
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);
			var transformedValue = replaceTextAttribute.Transform(value);
		}
	}
}
