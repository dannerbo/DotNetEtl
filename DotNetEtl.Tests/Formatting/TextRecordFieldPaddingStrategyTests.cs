using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Tests
{
	[TestClass]
	public class TextRecordFieldPaddingStrategyTests
	{
		[TestMethod]
		public void Pad_StringValue_ValueIsPaddedOnRight()
		{
			var record = new MockRecord() { StringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.StringField, fieldLength, property);

			Assert.AreEqual("test      ", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NumericValue_ValueIsPaddedOnLeft()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.IntField.ToString(), fieldLength, property);

			Assert.AreEqual("        10", paddedFieldValue);
		}
		
		[TestMethod]
		public void Pad_DateTimeValue_ValueIsPaddedOnLeft()
		{
			var record = new MockRecord() { DateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.DateTimeField.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("  20000101", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableNumericValue_ValueIsPaddedOnLeft()
		{
			var record = new MockRecord() { NullableIntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.NullableIntField.ToString(), fieldLength, property);

			Assert.AreEqual("        10", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableDateTimeValue_ValueIsPaddedOnLeft()
		{
			var record = new MockRecord() { NullableDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.NullableDateTimeField.Value.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("  20000101", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_StringValueWithRightJustification_ValueIsPaddedOnLeft()
		{
			var record = new MockRecord() { StringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy() { LeftJustifiyText = false };
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.StringField, fieldLength, property);

			Assert.AreEqual("      test", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NumericValueWithLeftJustification_ValueIsPaddedOnRight()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy() { RightJustifyNumbers = false };
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.IntField.ToString(), fieldLength, property);

			Assert.AreEqual("10        ", paddedFieldValue);
		}
		
		[TestMethod]
		public void Pad_DateTimeValueWithLeftJustification_ValueIsPaddedOnRight()
		{
			var record = new MockRecord() { DateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy() { RightJustifyDateTimes = false };
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.DateTimeField.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("20000101  ", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_StringValueWithCustomPadChar_ValueIsPaddedWithCustomPadChar()
		{
			var record = new MockRecord() { StringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy() { TextPaddingChar = 'X' };
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.StringField, fieldLength, property);

			Assert.AreEqual("testXXXXXX", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NumericValueWithCustomPadChar_ValueIsPaddedWithCustomPadChar()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy() { NumberPaddingChar = 'X' };
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.IntField.ToString(), fieldLength, property);

			Assert.AreEqual("XXXXXXXX10", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_DateTimeValueWithCustomPadChar_ValueIsPaddedWithCustomPadChar()
		{
			var record = new MockRecord() { DateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy() { DateTimePaddingChar = 'X' };
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.DateTimeField.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("XX20000101", paddedFieldValue);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Pad_NullValue_ExceptionIsThrown()
		{
			var record = new MockRecord();
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.StringField, fieldLength, null);
		}

		[TestMethod]
		public void Pad_ZeroFieldLength_ValueIsNotPadded()
		{
			var record = new MockRecord() { StringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var fieldLength = 0;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.StringField, fieldLength, property);

			Assert.AreEqual(record.StringField, paddedFieldValue);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void Pad_NegativeFieldLength_ExceptionIsThrown()
		{
			var record = new MockRecord() { StringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var fieldLength = -1;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.StringField, fieldLength, property);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Pad_NullPropertyInfo_ExceptionIsThrown()
		{
			var fieldValue = "test";
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(fieldValue, fieldLength, null);
		}
		
		[TestMethod]
		public void Pad_StringFieldWithLeftJustifyTextAttribute_ValueIsLeftJustified()
		{
			var record = new MockRecord() { LeftJustifiedStringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedStringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedStringField, fieldLength, property);

			Assert.AreEqual("test      ", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_IntFieldWithLeftJustifyTextAttribute_ValueIsLeftJustified()
		{
			var record = new MockRecord() { LeftJustifiedIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedIntField.ToString(), fieldLength, property);

			Assert.AreEqual("100       ", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableIntFieldWithLeftJustifyTextAttribute_ValueIsLeftJustified()
		{
			var record = new MockRecord() { LeftJustifiedNullableIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedNullableIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedNullableIntField.ToString(), fieldLength, property);

			Assert.AreEqual("100       ", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_DateTimeFieldWithLeftJustifyTextAttribute_ValueIsLeftJustified()
		{
			var record = new MockRecord() { LeftJustifiedDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedDateTimeField.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("20000101  ", paddedFieldValue);
		}
		
		[TestMethod]
		public void Pad_NullableDateTimeFieldWithLeftJustifyTextAttribute_ValueIsLeftJustified()
		{
			var record = new MockRecord() { LeftJustifiedNullableDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedNullableDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedNullableDateTimeField.Value.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("20000101  ", paddedFieldValue);
		}
		
		[TestMethod]
		public void Pad_StringFieldWithRightJustifyTextAttribute_ValueIsRightJustified()
		{
			var record = new MockRecord() { RightJustifiedStringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedStringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedStringField, fieldLength, property);

			Assert.AreEqual("      test", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_IntFieldWithRightJustifyTextAttribute_ValueIsRightJustified()
		{
			var record = new MockRecord() { RightJustifiedIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedIntField.ToString(), fieldLength, property);

			Assert.AreEqual("       100", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableIntFieldWithRightJustifyTextAttribute_ValueIsRightJustified()
		{
			var record = new MockRecord() { RightJustifiedNullableIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedNullableIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedNullableIntField.ToString(), fieldLength, property);

			Assert.AreEqual("       100", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_DateTimeFieldWithRightJustifyTextAttribute_ValueIsRightJustified()
		{
			var record = new MockRecord() { RightJustifiedDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedDateTimeField.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("  20000101", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableDateTimeFieldWithRightJustifyTextAttribute_ValueIsRightJustified()
		{
			var record = new MockRecord() { RightJustifiedNullableDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedNullableDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedNullableDateTimeField.Value.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("  20000101", paddedFieldValue);
		}
		
		[TestMethod]
		public void Pad_StringFieldWithLeftJustifyTextAttributeAndCustomPaddingChar_ValueIsLeftJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { LeftJustifiedWithCustomPaddingCharStringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedWithCustomPaddingCharStringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedWithCustomPaddingCharStringField, fieldLength, property);

			Assert.AreEqual("testXXXXXX", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_IntFieldWithLeftJustifyTextAttributeAndCustomPaddingChar_ValueIsLeftJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { LeftJustifiedWithCustomPaddingCharIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedWithCustomPaddingCharIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedWithCustomPaddingCharIntField.ToString(), fieldLength, property);

			Assert.AreEqual("100XXXXXXX", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableIntFieldWithLeftJustifyTextAttributeAndCustomPaddingChar_ValueIsLeftJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { LeftJustifiedWithCustomPaddingCharNullableIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedWithCustomPaddingCharNullableIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedWithCustomPaddingCharNullableIntField.ToString(), fieldLength, property);

			Assert.AreEqual("100XXXXXXX", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_DateTimeFieldWithLeftJustifyTextAttributeAndCustomPaddingChar_ValueIsLeftJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { LeftJustifiedWithCustomPaddingCharDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedWithCustomPaddingCharDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedWithCustomPaddingCharDateTimeField.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("20000101XX", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableDateTimeFieldWithLeftJustifyTextAttributeAndCustomPaddingChar_ValueIsLeftJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { LeftJustifiedWithCustomPaddingCharNullableDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.LeftJustifiedWithCustomPaddingCharNullableDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.LeftJustifiedWithCustomPaddingCharNullableDateTimeField.Value.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("20000101XX", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_StringFieldWithRightJustifyTextAttributeAndCustomPaddingChar_ValueIsRightJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { RightJustifiedWithCustomPaddingCharStringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedWithCustomPaddingCharStringField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedWithCustomPaddingCharStringField, fieldLength, property);

			Assert.AreEqual("XXXXXXtest", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_IntFieldWithRightJustifyTextAttributeAndCustomPaddingChar_ValueIsRightJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { RightJustifiedWithCustomPaddingCharIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedWithCustomPaddingCharIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedWithCustomPaddingCharIntField.ToString(), fieldLength, property);

			Assert.AreEqual("XXXXXXX100", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableIntFieldWithRightJustifyTextAttributeAndCustomPaddingChar_ValueIsRightJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { RightJustifiedWithCustomPaddingCharNullableIntField = 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedWithCustomPaddingCharNullableIntField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedWithCustomPaddingCharNullableIntField.ToString(), fieldLength, property);

			Assert.AreEqual("XXXXXXX100", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_DateTimeFieldWithRightJustifyTextAttributeAndCustomPaddingChar_ValueIsRightJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { RightJustifiedWithCustomPaddingCharDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedWithCustomPaddingCharDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedWithCustomPaddingCharDateTimeField.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("XX20000101", paddedFieldValue);
		}

		[TestMethod]
		public void Pad_NullableDateTimeFieldWithRightJustifyTextAttributeAndCustomPaddingChar_ValueIsRightJustifiedWithCustomPaddingChar()
		{
			var record = new MockRecord() { RightJustifiedWithCustomPaddingCharNullableDateTimeField = DateTime.Parse("2000-01-01") };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.RightJustifiedWithCustomPaddingCharNullableDateTimeField));
			var fieldLength = 10;
			var fieldPaddingStrategy = new TextRecordFieldPaddingStrategy();
			var paddedFieldValue = fieldPaddingStrategy.Pad(record.RightJustifiedWithCustomPaddingCharNullableDateTimeField.Value.ToString("yyyyMMdd"), fieldLength, property);

			Assert.AreEqual("XX20000101", paddedFieldValue);
		}

		public class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
			public DateTime DateTimeField { get; set; }
			public int? NullableIntField { get; set; }
			public DateTime? NullableDateTimeField { get; set; }

			[LeftJustifyText]
			public string LeftJustifiedStringField { get; set; }
			[LeftJustifyText]
			public int LeftJustifiedIntField { get; set; }
			[LeftJustifyText]
			public DateTime LeftJustifiedDateTimeField { get; set; }
			[LeftJustifyText]
			public int? LeftJustifiedNullableIntField { get; set; }
			[LeftJustifyText]
			public DateTime? LeftJustifiedNullableDateTimeField { get; set; }
			
			[RightJustifyText]
			public string RightJustifiedStringField { get; set; }
			[RightJustifyText]
			public int RightJustifiedIntField { get; set; }
			[RightJustifyText]
			public DateTime RightJustifiedDateTimeField { get; set; }
			[RightJustifyText]
			public int? RightJustifiedNullableIntField { get; set; }
			[RightJustifyText]
			public DateTime? RightJustifiedNullableDateTimeField { get; set; }

			[LeftJustifyText('X')]
			public string LeftJustifiedWithCustomPaddingCharStringField { get; set; }
			[LeftJustifyText('X')]
			public int LeftJustifiedWithCustomPaddingCharIntField { get; set; }
			[LeftJustifyText('X')]
			public DateTime LeftJustifiedWithCustomPaddingCharDateTimeField { get; set; }
			[LeftJustifyText('X')]
			public int? LeftJustifiedWithCustomPaddingCharNullableIntField { get; set; }
			[LeftJustifyText('X')]
			public DateTime? LeftJustifiedWithCustomPaddingCharNullableDateTimeField { get; set; }

			[RightJustifyText('X')]
			public string RightJustifiedWithCustomPaddingCharStringField { get; set; }
			[RightJustifyText('X')]
			public int RightJustifiedWithCustomPaddingCharIntField { get; set; }
			[RightJustifyText('X')]
			public DateTime RightJustifiedWithCustomPaddingCharDateTimeField { get; set; }
			[RightJustifyText('X')]
			public int? RightJustifiedWithCustomPaddingCharNullableIntField { get; set; }
			[RightJustifyText('X')]
			public DateTime? RightJustifiedWithCustomPaddingCharNullableDateTimeField { get; set; }
		}
	}
}
