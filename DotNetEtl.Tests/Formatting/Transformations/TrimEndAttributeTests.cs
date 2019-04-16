using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class TrimEndAttributeTests
	{
		[TestMethod]
		public void Transform_TextWithLeadingAndTrailingSpaces_TrimmedTextReturned()
		{
			var value = " test ";
			var trimEndAttribute = new TrimEndAttribute();
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual(" test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithLeadingSpaces_OriginalTextReturned()
		{
			var value = " test";
			var trimEndAttribute = new TrimEndAttribute();
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual(" test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithTrailingSpaces_TrimmedTextReturned()
		{
			var value = "test ";
			var trimEndAttribute = new TrimEndAttribute();
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithNoLeadingOrTrailingSpaces_OriginalTextReturned()
		{
			var value = "test";
			var trimEndAttribute = new TrimEndAttribute();
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_EmptyText_EmptyTextReturned()
		{
			var value = String.Empty;
			var trimEndAttribute = new TrimEndAttribute();
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual(String.Empty, transformedValue);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void Transform_NullValue_EmptyTextReturned()
		{
			var trimEndAttribute = new TrimEndAttribute();
			var transformedValue = trimEndAttribute.Transform(null);
		}

		[TestMethod]
		public void Transform_TextWithTrailingSpacesAndNonSpaceTrimChar_OriginalTextReturned()
		{
			var value = "test ";
			var trimEndAttribute = new TrimEndAttribute('0');
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual("test ", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithTrailingZeroesAndZeroTrimChar_TrimmedTextReturned()
		{
			var value = "test0";
			var trimEndAttribute = new TrimEndAttribute('0');
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithMultipleButDifferentLeadingAndTrailingTrimmableChars_TrimmedTextReturned()
		{
			var value = " 0 test 0 ";
			var trimEndAttribute = new TrimEndAttribute('0', ' ');
			var transformedValue = trimEndAttribute.Transform(value);

			Assert.AreEqual(" 0 test", transformedValue);
		}
	}
}
