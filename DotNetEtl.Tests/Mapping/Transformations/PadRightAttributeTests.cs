using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class PadRightAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_NonEmptyValueUnderTotalWidth_ValueIsPadded()
		{
			var record = new MockRecord() { StringField = "ABC" };
			var totalWidth = 5;
			var paddingChar = ' ';
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);

			padRightAttribute.ApplyTransform(property, record);

			Assert.AreEqual("ABC  ", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_NonEmptyValueAtTotalWidth_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = "ABCDE" };
			var totalWidth = 5;
			var paddingChar = ' ';
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);

			padRightAttribute.ApplyTransform(property, record);

			Assert.AreEqual("ABCDE", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_NonEmptyValueOverTotalWidth_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = "ABCDEFG" };
			var totalWidth = 5;
			var paddingChar = ' ';
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);

			padRightAttribute.ApplyTransform(property, record);

			Assert.AreEqual("ABCDEFG", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_EmptyValue_ValueIsPadded()
		{
			var record = new MockRecord() { StringField = "" };
			var totalWidth = 5;
			var paddingChar = ' ';
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);

			padRightAttribute.ApplyTransform(property, record);

			Assert.AreEqual("     ", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_NullValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = null };
			var totalWidth = 5;
			var paddingChar = ' ';
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);

			padRightAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.StringField);
		}
		
		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ApplyTransform_NonStringField_ExceptionIsThrown()
		{
			var record = new MockRecord() { IntField = 1 };
			var totalWidth = 5;
			var paddingChar = ' ';
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var padRightAttribute = new PadRightAttribute(totalWidth, paddingChar);

			padRightAttribute.ApplyTransform(property, record);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
		}
	}
}
