using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class TruncateTextAttributeTests
	{
		[TestMethod]
		public void Transform_TextOverMaxLength_TruncatedTextReturned()
		{
			var value = "123456";
			var maxLength = 5;
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);
			var transformedValue = truncateTextAttribute.Transform(value);

			Assert.AreEqual("12345", transformedValue);
		}

		[TestMethod]
		public void Transform_TextUnderMaxLength_OriginalTextReturned()
		{
			var value = "123";
			var maxLength = 5;
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);
			var transformedValue = truncateTextAttribute.Transform(value);

			Assert.AreEqual("123", transformedValue);
		}
		
		[TestMethod]
		public void Transform_TextAtMaxLength_OriginalTextReturned()
		{
			var value = "12345";
			var maxLength = 5;
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);
			var transformedValue = truncateTextAttribute.Transform(value);

			Assert.AreEqual("12345", transformedValue);
		}
		
		[TestMethod]
		public void Transform_EmptyString_EmptyStringReturned()
		{
			var value = String.Empty;
			var maxLength = 5;
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);
			var transformedValue = truncateTextAttribute.Transform(value);

			Assert.AreEqual(String.Empty, transformedValue);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void Transform_NullValue_ExceptionIsThrown()
		{
			var maxLength = 5;
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);
			var transformedValue = truncateTextAttribute.Transform(null);
		}
		
		[TestMethod]
		public void Transform_ZeroMaxLength_EmptyStringReturned()
		{
			var value = "test";
			var maxLength = 0;
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);
			var transformedValue = truncateTextAttribute.Transform(value);

			Assert.AreEqual(String.Empty, transformedValue);
		}
		
		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Transform_NegativeMaxLength_ExceptionIsThrown()
		{
			var value = "test";
			var maxLength = -1;
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);
			var transformedValue = truncateTextAttribute.Transform(value);
		}
	}
}
