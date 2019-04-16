using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class TruncateTextAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_StringFieldWithValueOverMaxLength_TextIsTruncated()
		{
			var maxLength = 5;
			var record = new MockRecord() { StringField = "123456789" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);

			truncateTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("12345", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithValueAtMaxLength_TextIsUnchanged()
		{
			var maxLength = 9;
			var record = new MockRecord() { StringField = "123456789" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);

			truncateTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("123456789", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithValueUnderMaxLength_TextIsUnchanged()
		{
			var maxLength = 9;
			var record = new MockRecord() { StringField = "12345" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);

			truncateTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("12345", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithEmptyValue_ValueIsUnchanged()
		{
			var maxLength = 5;
			var record = new MockRecord() { StringField = "" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);

			truncateTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithNullValue_ValueIsUnchanged()
		{
			var maxLength = 5;
			var record = new MockRecord() { StringField = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);

			truncateTextAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.StringField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ApplyTransform_IntField_ExceptionIsThrown()
		{
			var maxLength = 5;
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var truncateTextAttribute = new TruncateTextAttribute(maxLength);

			truncateTextAttribute.ApplyTransform(property, record);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
		}
	}
}
