using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Tests
{
	[TestClass]
	public class TextRecordFieldFormatterTests
	{
		[TestMethod]
		public void Format_StringFieldWithNoAttributes_FieldIsNotFormatted()
		{
			var record = new MockRecord() { StringField = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringField, property);

			Assert.AreEqual(record.StringField, formattedValue);
		}

		[TestMethod]
		public void Format_NonNullOrEmptyStringFieldWithToStringAttribute_FieldIsConvertedToStringViaAttribute()
		{
			var record = new MockRecord() { StringFieldWithToStringAttribute = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithToStringAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithToStringAttribute, property);

			Assert.AreEqual($"'{record.StringFieldWithToStringAttribute}'", formattedValue);
		}

		[TestMethod]
		public void Format_NonNullOrEmptyStringFieldWithToStringAndTransformAttributes_FieldIsConvertedToStringViaAttributeAndFormatted()
		{
			var record = new MockRecord() { StringFieldWithToStringAndTransformAttributes = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithToStringAndTransformAttributes));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithToStringAndTransformAttributes, property);

			Assert.AreEqual($"|'{record.StringFieldWithToStringAndTransformAttributes}'|", formattedValue);
		}

		[TestMethod]
		public void Format_EmptyStringFieldWithToStringAttribute_FieldIsConvertedToStringViaAttribute()
		{
			var record = new MockRecord() { StringFieldWithToStringAttribute = String.Empty };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithToStringAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithToStringAttribute, property);

			Assert.AreEqual($"'{record.StringFieldWithToStringAttribute}'", formattedValue);
		}

		[TestMethod]
		public void Format_EmptyStringFieldWithToStringAndTransformAttributes_FieldIsConvertedToStringViaAttributeAndFormatted()
		{
			var record = new MockRecord() { StringFieldWithToStringAndTransformAttributes = String.Empty };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithToStringAndTransformAttributes));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithToStringAndTransformAttributes, property);

			Assert.AreEqual($"|'{record.StringFieldWithToStringAndTransformAttributes}'|", formattedValue);
		}

		[TestMethod]
		public void Format_NullStringFieldWithToStringAndTransformAttributes_FieldIsConvertedToStringViaAttributeAndFormatted()
		{
			var record = new MockRecord() { StringFieldWithToStringAndTransformAttributes = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithToStringAndTransformAttributes));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithToStringAndTransformAttributes, property);

			Assert.AreEqual($"|''|", formattedValue);
		}

		[TestMethod]
		public void Format_NullStringFieldWithTransformAttribute_FormattedValueIsNull()
		{
			var record = new MockRecord() { StringFieldWithTransformAttribute = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithTransformAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithTransformAttribute, property);

			Assert.IsNull(formattedValue);
		}

		[TestMethod]
		public void Format_NullStringFieldWithNullableTransformAttribute_FieldIsFormatted()
		{
			var record = new MockRecord() { StringFieldWithNullableTransformAttribute = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithNullableTransformAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithNullableTransformAttribute, property);

			Assert.AreEqual($"|NULL|", formattedValue);
		}

		[TestMethod]
		public void Format_IntFieldWithNoAttributes_FieldIsConvertedToStringAndNotFormatted()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.IntField, property);

			Assert.AreEqual("10", formattedValue);
		}

		[TestMethod]
		public void Format_NonNullOrEmptyIntFieldWithToStringAttribute_FieldIsConvertedToStringViaAttribute()
		{
			var record = new MockRecord() { IntFieldWithToStringAttribute = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntFieldWithToStringAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.IntFieldWithToStringAttribute, property);

			Assert.AreEqual($"'{record.IntFieldWithToStringAttribute}'", formattedValue);
		}

		[TestMethod]
		public void Format_NonNullOrEmptyIntFieldWithToStringAndTransformAttributes_FieldIsConvertedToStringViaAttributeAndFormatted()
		{
			var record = new MockRecord() { IntFieldWithToStringAndTransformAttributes = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntFieldWithToStringAndTransformAttributes));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.IntFieldWithToStringAndTransformAttributes, property);

			Assert.AreEqual($"|'{record.IntFieldWithToStringAndTransformAttributes}'|", formattedValue);
		}

		[TestMethod]
		public void Format_NullableIntFieldWithNoAttributes_FieldIsConvertedToStringAndNotFormatted()
		{
			var record = new MockRecord() { NullableIntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntField));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.NullableIntField, property);

			Assert.AreEqual("10", formattedValue);
		}

		[TestMethod]
		public void Format_NonNullOrEmptyNullableIntFieldWithToStringAttribute_FieldIsConvertedToStringViaAttribute()
		{
			var record = new MockRecord() { NullableIntFieldWithToStringAttribute = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntFieldWithToStringAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.NullableIntFieldWithToStringAttribute, property);

			Assert.AreEqual($"'{record.NullableIntFieldWithToStringAttribute}'", formattedValue);
		}

		[TestMethod]
		public void Format_NonNullOrEmptyNullableIntFieldWithToStringAndTransformAttributes_FieldIsConvertedToStringViaAttributeAndFormatted()
		{
			var record = new MockRecord() { NullableIntFieldWithToStringAndTransformAttributes = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntFieldWithToStringAndTransformAttributes));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.NullableIntFieldWithToStringAndTransformAttributes, property);

			Assert.AreEqual($"|'{record.NullableIntFieldWithToStringAndTransformAttributes}'|", formattedValue);
		}

		[TestMethod]
		public void Format_NullNullableIntFieldWithToStringAndTransformAttributes_FieldIsConvertedToStringViaAttributeAndFormatted()
		{
			var record = new MockRecord() { NullableIntFieldWithToStringAndTransformAttributes = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntFieldWithToStringAndTransformAttributes));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.NullableIntFieldWithToStringAndTransformAttributes, property);

			Assert.AreEqual($"|''|", formattedValue);
		}

		[TestMethod]
		public void Format_NullNullableIntFieldWithTransformAttribute_FormattedValueIsNull()
		{
			var record = new MockRecord() { NullableIntFieldWithTransformAttribute = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntFieldWithTransformAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.NullableIntFieldWithTransformAttribute, property);

			Assert.IsNull(formattedValue);
		}

		[TestMethod]
		public void Format_NullNullableIntFieldWithNullableTransformAttribute_FieldIsFormatted()
		{
			var record = new MockRecord() { NullableIntFieldWithNullableTransformAttribute = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntFieldWithNullableTransformAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.NullableIntFieldWithNullableTransformAttribute, property);

			Assert.AreEqual($"|NULL|", formattedValue);
		}
		
		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Format_ToStringAttributeThrowsException_ExceptionIsPropogated()
		{
			var record = new MockRecord() { StringFieldWithErroredToStringAttribute = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithErroredToStringAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithErroredToStringAttribute, property);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Format_TransformStringAttributeThrowsException_ExceptionIsPropogated()
		{
			var record = new MockRecord() { StringFieldWithErroredTransformStringAttribute = "test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringFieldWithErroredTransformStringAttribute));
			var formatter = new TextRecordFieldFormatter();
			var formattedValue = formatter.Format(record.StringFieldWithErroredTransformStringAttribute, property);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			[MockToString("'")]
			public string StringFieldWithToStringAttribute { get; set; }
			[MockToString("'")]
			[MockTransformString("|", false)]
			public string StringFieldWithToStringAndTransformAttributes { get; set; }
			[MockTransformString("|", true)]
			public string StringFieldWithNullableTransformAttribute { get; set; }
			[MockTransformString("|", false)]
			public string StringFieldWithTransformAttribute { get; set; }

			public int IntField { get; set; }
			[MockToString("'")]
			public int IntFieldWithToStringAttribute { get; set; }
			[MockToString("'")]
			[MockTransformString("|", false)]
			public int IntFieldWithToStringAndTransformAttributes { get; set; }
			[MockTransformString("|", true)]
			public int IntFieldWithNullableTransformAttribute { get; set; }
			[MockTransformString("|", false)]
			public int IntFieldWithTransformAttribute { get; set; }

			public int? NullableIntField { get; set; }
			[MockToString("'")]
			public int? NullableIntFieldWithToStringAttribute { get; set; }
			[MockToString("'")]
			[MockTransformString("|", false)]
			public int? NullableIntFieldWithToStringAndTransformAttributes { get; set; }
			[MockTransformString("|", true)]
			public int? NullableIntFieldWithNullableTransformAttribute { get; set; }
			[MockTransformString("|", false)]
			public int? NullableIntFieldWithTransformAttribute { get; set; }

			[MockErroredToString]
			public string StringFieldWithErroredToStringAttribute { get; set; }
			[MockErroredTransformString]
			public string StringFieldWithErroredTransformStringAttribute { get; set; }
		}

		private class MockToStringAttribute : ToStringAttribute
		{
			private string qualifier;

			public MockToStringAttribute(string qualifier)
			{
				this.qualifier = qualifier;
			}

			public override string ToString(object fieldValue)
			{
				return $"{this.qualifier}{fieldValue}{this.qualifier}";
			}
		}

		private class MockTransformStringAttribute : TransformStringAttribute
		{
			private string qualifier;
			private bool canTransformNullValue;

			public MockTransformStringAttribute(string qualifier, bool canTransformNullValue)
			{
				this.qualifier = qualifier;
				this.canTransformNullValue = canTransformNullValue;
			}

			public override bool CanTransformNullValue => this.canTransformNullValue;

			public override string Transform(string value)
			{
				if (value == null && !this.CanTransformNullValue)
				{
					throw new InvalidOperationException($"An attempt was made to transform a null value when {nameof(this.CanTransformNullValue)} is false.");
				}

				return value != null
					? $"{this.qualifier}{value}{this.qualifier}"
					: $"{this.qualifier}NULL{this.qualifier}";
			}
		}

		private class MockErroredToStringAttribute : ToStringAttribute
		{
			public override string ToString(object fieldValue)
			{
				throw new InternalTestFailureException();
			}
		}

		private class MockErroredTransformStringAttribute : TransformStringAttribute
		{
			public override string Transform(string value)
			{
				throw new InternalTestFailureException();
			}
		}
	}
}
