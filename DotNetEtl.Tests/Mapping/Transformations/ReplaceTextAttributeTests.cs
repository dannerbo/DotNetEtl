using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class ReplaceTextAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_OldValueExistsAndSameLengthAsReplacement_TextIsReplaced()
		{
			var record = new MockRecord() { StringField = "ABC" };
			var oldValue = "A";
			var newValue = "X";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);

			replaceTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("XBC", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_OldValueExistsAndNotSameLengthAsReplacement_TextIsReplaced()
		{
			var record = new MockRecord() { StringField = "ABC" };
			var oldValue = "A";
			var newValue = "XX";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);

			replaceTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("XXBC", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_OldValueExistsAndReplacementIsEmptyString_TextIsReplaced()
		{
			var record = new MockRecord() { StringField = "ABC" };
			var oldValue = "A";
			var newValue = "";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);

			replaceTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("BC", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_OldValueDoesNotExist_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = "ABC" };
			var oldValue = "X";
			var newValue = "Y";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);

			replaceTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("ABC", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_OldValueExistsMultipleTimes_TextIsReplaced()
		{
			var record = new MockRecord() { StringField = "ABACA" };
			var oldValue = "A";
			var newValue = "X";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);

			replaceTextAttribute.ApplyTransform(property, record);

			Assert.AreEqual("XBXCX", record.StringField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ApplyTransform_NonStringField_ExceptionIsThrown()
		{
			var record = new MockRecord() { IntField = 1 };
			var oldValue = "A";
			var newValue = "X";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);

			replaceTextAttribute.ApplyTransform(property, record);
		}



		[TestMethod]
		public void ApplyTransform_NullValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = null };
			var oldValue = "A";
			var newValue = "X";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var replaceTextAttribute = new ReplaceTextAttribute(oldValue, newValue);

			replaceTextAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.StringField);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
		}
	}
}
