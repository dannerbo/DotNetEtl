using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class TrimAttributeTests
	{
		[TestMethod]
		public void Transform_TextWithLeadingAndTrailingSpaces_TrimmedTextReturned()
		{
			var value = " test ";
			var trimAttribute = new TrimAttribute();
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithLeadingSpaces_TrimmedTextReturned()
		{
			var value = " test";
			var trimAttribute = new TrimAttribute();
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithTrailingSpaces_TrimmedTextReturned()
		{
			var value = "test ";
			var trimAttribute = new TrimAttribute();
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithNoLeadingOrTrailingSpaces_OriginalTextReturned()
		{
			var value = "test";
			var trimAttribute = new TrimAttribute();
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_EmptyText_EmptyTextReturned()
		{
			var value = String.Empty;
			var trimAttribute = new TrimAttribute();
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual(String.Empty, transformedValue);
		}

		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void Transform_NullValue_EmptyTextReturned()
		{
			var trimAttribute = new TrimAttribute();
			var transformedValue = trimAttribute.Transform(null);
		}

		[TestMethod]
		public void Transform_TextWithLeadingSpacesAndNonSpaceTrimChar_OriginalTextReturned()
		{
			var value = " test";
			var trimAttribute = new TrimAttribute('0');
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual(" test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithLeadingZeroesAndZeroTrimChar_TrimmedTextReturned()
		{
			var value = "0test";
			var trimAttribute = new TrimAttribute('0');
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}

		[TestMethod]
		public void Transform_TextWithMultipleButDifferentLeadingAndTrailingTrimmableChars_TrimmedTextReturned()
		{
			var value = "0 test 0 ";
			var trimAttribute = new TrimAttribute('0', ' ');
			var transformedValue = trimAttribute.Transform(value);

			Assert.AreEqual("test", transformedValue);
		}
	}
}
