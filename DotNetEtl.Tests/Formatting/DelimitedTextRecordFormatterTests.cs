using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Formatting.Tests
{
	[TestClass]
	public class DelimitedTextRecordFormatterTests
	{
		[TestMethod]
		public void Constructor_DelimiterIsNotProvided_DelimiterPropertyEqualsComma()
		{
			var formatter = new DelimitedTextRecordFormatter();

			Assert.AreEqual(",", formatter.Delimiter);
		}

		[TestMethod]
		public void Constructor_DelimiterIsProvided_DelimiterPropertyEqualsProvidedDelimiter()
		{
			var formatter = new DelimitedTextRecordFormatter("|");

			Assert.AreEqual("|", formatter.Delimiter);
		}

		[TestMethod]
		public void Format_RecordWithAllPropertiesSet_RecordFormattedAsTextWithDelimiters()
		{
			var formatter = new DelimitedTextRecordFormatter();
			var record = new MockRecord("text", 10, -1);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("text,10,-1", formattedRecord);
		}

		[TestMethod]
		public void Format_RecordWithNullProperty_RecordFormattedAsTextWithDelimiters()
		{
			var formatter = new DelimitedTextRecordFormatter();
			var record = new MockRecord("text", 10, null);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("text,10,", formattedRecord);
		}

		[TestMethod]
		public void Format_RecordWithNoDestinationFieldOrdinalAttributes_RecordFormattedAsEmptyString()
		{
			var formatter = new DelimitedTextRecordFormatter();
			var record = new { StringField = "text", IntField = 10, NullableIntField = -1 };
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual(String.Empty, formattedRecord);
		}

		[TestMethod]
		public void Format_RecordWithAllPropertiesSetAndFieldQualifierProvided_AllFieldsQualified()
		{
			var formatter = new DelimitedTextRecordFormatter();

			formatter.FieldQualifier = "'";

			var record = new MockRecord("text", 10, -1);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("'text','10','-1'", formattedRecord);
		}

		[TestMethod]
		public void Format_RecordWithNullPropertyAndFieldQualifierProvided_AllFieldsQualified()
		{
			var formatter = new DelimitedTextRecordFormatter();

			formatter.FieldQualifier = "'";

			var record = new MockRecord("text", 10, null);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("'text','10',''", formattedRecord);
		}

		[TestMethod]
		public void Format_DelimiterNotInAnyFieldAndConditionalFieldQualifierProvided_NoFieldsQualified()
		{
			var formatter = new DelimitedTextRecordFormatter();

			formatter.FieldQualifier = "'";
			formatter.IsFieldQualifierConditional = true;

			var record = new MockRecord("text", 10, -1);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("text,10,-1", formattedRecord);
		}

		[TestMethod]
		public void Format_DelimiterNotInAnyFieldAndOneNullPropertyAndConditionalFieldQualifierProvided_NoFieldsQualified()
		{
			var formatter = new DelimitedTextRecordFormatter();

			formatter.FieldQualifier = "'";
			formatter.IsFieldQualifierConditional = true;

			var record = new MockRecord("text", 10, null);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("text,10,", formattedRecord);
		}

		[TestMethod]
		public void Format_DelimiterInFieldAndConditionalFieldQualifierProvided_FieldContainingQualifierIsQualified()
		{
			var formatter = new DelimitedTextRecordFormatter();

			formatter.FieldQualifier = "'";
			formatter.IsFieldQualifierConditional = true;

			var record = new MockRecord("te,xt", 10, null);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("'te,xt',10,", formattedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Format_RecordWithAllPropertiesSetAndMissingDestinationFieldOrdinal_ExceptionIsThrown()
		{
			var formatter = new DelimitedTextRecordFormatter();
			var record = new MockRecordWithMissingDestinationFieldOrdinal("text", 10, -1);
			var formattedRecord = formatter.Format(record);
		}

		[TestMethod]
		public void Format_CustomDestinationFieldOrdinalProvider_AllFieldsInquiriedAndRecordIsFormatted()
		{
			var destinationFieldOrdinalProvider = MockRepository.GenerateMock<IDestinationFieldOrdinalProvider>();
			var formatter = new DelimitedTextRecordFormatter(destinationFieldOrdinalProvider: destinationFieldOrdinalProvider);
			var record = new MockRecord("text", 10, -1);

			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)), Arg<object>.Is.Anything, out Arg<int>.Out(0).Dummy)).Return(true).Repeat.Once();
			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)), Arg<object>.Is.Anything, out Arg<int>.Out(1).Dummy)).Return(true).Repeat.Once();
			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)), Arg<object>.Is.Anything, out Arg<int>.Out(2).Dummy)).Return(true).Repeat.Once();
			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)), Arg<object>.Is.Anything, out Arg<int>.Out(-1).Dummy)).Return(false).Repeat.Once();

			var formattedRecord = formatter.Format(record);

			destinationFieldOrdinalProvider.VerifyAllExpectations();

			Assert.AreEqual("text,10,-1", formattedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Format_DestinationFieldOrdinalProviderThrowsException_ExceptionIsPropogated()
		{
			var destinationFieldOrdinalProvider = MockRepository.GenerateMock<IDestinationFieldOrdinalProvider>();
			var formatter = new DelimitedTextRecordFormatter(destinationFieldOrdinalProvider: destinationFieldOrdinalProvider);
			var record = new MockRecord("text", 10, -1);

			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)), Arg<object>.Is.Anything, out Arg<int>.Out(0).Dummy)).Return(true).Throw(new InternalTestFailureException());
			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)), Arg<object>.Is.Anything, out Arg<int>.Out(1).Dummy)).Return(true).Throw(new InternalTestFailureException());
			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)), Arg<object>.Is.Anything, out Arg<int>.Out(2).Dummy)).Return(true).Throw(new InternalTestFailureException());
			destinationFieldOrdinalProvider.Expect(x => x.TryGetDestinationFieldOrdinal(Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)), Arg<object>.Is.Anything, out Arg<int>.Out(-1).Dummy)).Return(false).Throw(new InternalTestFailureException());

			var formattedRecord = formatter.Format(record);
		}

		[TestMethod]
		public void Format_CustomTextRecordFieldFormatter_CustomFieldFormatterUsedAndRecordIsFormatted()
		{
			var textRecordFieldFormatter = MockRepository.GenerateMock<ITextRecordFieldFormatter>();
			var formatter = new DelimitedTextRecordFormatter(fieldFormatter: textRecordFieldFormatter);
			var record = new MockRecord("text", 10, -1);

			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal("text"), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)))).Return("text").Repeat.Once();
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(10), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)))).Return("10").Repeat.Once();
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(-1), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)))).Return("-1").Repeat.Once();
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)))).Repeat.Never();

			var formattedRecord = formatter.Format(record);

			textRecordFieldFormatter.VerifyAllExpectations();

			Assert.AreEqual("text,10,-1", formattedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Format_TextRecordFieldFormatterThrowsException_ExceptionIsPropogated()
		{
			var textRecordFieldFormatter = MockRepository.GenerateMock<ITextRecordFieldFormatter>();
			var formatter = new DelimitedTextRecordFormatter(fieldFormatter: textRecordFieldFormatter);
			var record = new MockRecord("text", 10, -1);

			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal("text"), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)))).Throw(new InternalTestFailureException());
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(10), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)))).Throw(new InternalTestFailureException());
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(-1), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)))).Throw(new InternalTestFailureException());
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)))).Repeat.Never();

			var formattedRecord = formatter.Format(record);
		}

		[TestMethod]
		public void Format_RecordWithTransformableProperty_TransformablePropertyIsTransformed()
		{
			var formatter = new DelimitedTextRecordFormatter();
			var record = new MockRecordWithTransform("text", 10, -1);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("transformed,10,-1", formattedRecord);
		}

		public class MockRecord
		{
			public MockRecord(string stringField, int intField, int? nullableIntField)
			{
				this.StringField = stringField;
				this.IntField = intField;
				this.NullableIntField = nullableIntField;
			}

			public MockRecord()
			{
			}

			[DestinationFieldOrdinal(0)]
			public string StringField { get; set; }
			[DestinationFieldOrdinal(1)]
			public int IntField { get; set; }
			[DestinationFieldOrdinal(2)]
			public int? NullableIntField { get; set; }
			public string NotUsed { get; set; }
		}

		private class MockRecordWithMissingDestinationFieldOrdinal
		{
			public MockRecordWithMissingDestinationFieldOrdinal(string stringField, int intField, int? nullableIntField)
			{
				this.StringField = stringField;
				this.IntField = intField;
				this.NullableIntField = nullableIntField;
			}

			public MockRecordWithMissingDestinationFieldOrdinal()
			{
			}

			[DestinationFieldOrdinal(0)]
			public string StringField { get; set; }
			[DestinationFieldOrdinal(1)]
			public int IntField { get; set; }
			[DestinationFieldOrdinal(3)]
			public int? NullableIntField { get; set; }
			public string NotUsed { get; set; }
		}

		private class MockRecordWithTransform
		{
			public MockRecordWithTransform(string stringField, int intField, int? nullableIntField)
			{
				this.StringField = stringField;
				this.IntField = intField;
				this.NullableIntField = nullableIntField;
			}

			public MockRecordWithTransform()
			{
			}

			[DestinationFieldOrdinal(0)]
			[MockTransformString("transformed")]
			public string StringField { get; set; }
			[DestinationFieldOrdinal(1)]
			public int IntField { get; set; }
			[DestinationFieldOrdinal(2)]
			public int? NullableIntField { get; set; }
			public string NotUsed { get; set; }
		}

		private class MockTransformStringAttribute : TransformStringAttribute
		{
			private string valueToReturn;

			public MockTransformStringAttribute(string valueToReturn)
			{
				this.valueToReturn = valueToReturn;
			}

			public override string Transform(string value)
			{
				return this.valueToReturn;
			}
		}
	}
}
