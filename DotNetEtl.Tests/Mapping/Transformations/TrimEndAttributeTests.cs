using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class TrimEndAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_StringFieldWithSpacesOnLeftAndRight_TextIsTrimmedAtEnd()
		{
			var record = new MockRecord() { StringField = "  test  " };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var trimEndAttribute = new TrimEndAttribute();

			trimEndAttribute.ApplyTransform(property, record);

			Assert.AreEqual("  test", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithCustomTrimCharsOnLeftAndRight_TextIsTrimmed()
		{
			var trimChars = new char[] { 'X', 'Y' };
			var record = new MockRecord() { StringField = "XYtestXY" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var trimEndAttribute = new TrimEndAttribute(trimChars);

			trimEndAttribute.ApplyTransform(property, record);

			Assert.AreEqual("XYtest", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithEmptyValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = "" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var trimEndAttribute = new TrimEndAttribute();

			trimEndAttribute.ApplyTransform(property, record);

			Assert.AreEqual("", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithOnlySpaces_ValueIsTrimmedToEmpty()
		{
			var record = new MockRecord() { StringField = "   " };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var trimEndAttribute = new TrimEndAttribute();

			trimEndAttribute.ApplyTransform(property, record);

			Assert.AreEqual("", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithNullValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var trimEndAttribute = new TrimEndAttribute();

			trimEndAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.StringField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ApplyTransform_IntField_ExceptionIsThrown()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var trimEndAttribute = new TrimEndAttribute();

			trimEndAttribute.ApplyTransform(property, record);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
		}
	}
}
