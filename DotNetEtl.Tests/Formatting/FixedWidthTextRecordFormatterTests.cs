using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Formatting.Tests
{
	[TestClass]
	public class FixedWidthTextRecordFormatterTests
	{
		[TestMethod]
		public void Format_DefaultDependencies_RecordIsFormatted()
		{
			var formatter = new FixedWidthTextRecordFormatter();
			var record = new MockRecord("text", 10, -1);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("text            10      -1", formattedRecord);
		}

		[TestMethod]
		public void Format_RecordWithNullPropertyAndDefaultDependencies_RecordIsFormatted()
		{
			var formatter = new FixedWidthTextRecordFormatter();
			var record = new MockRecord("text", 10, null);
			var formattedRecord = formatter.Format(record);

			Assert.AreEqual("text            10        ", formattedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Format_RecordWithMissingDestinationFieldLayout_ExceptionIsThrown()
		{
			var formatter = new FixedWidthTextRecordFormatter();
			var record = new MockRecordWithMissingDestinationFieldLayout("text", 10, -1);
			var formattedRecord = formatter.Format(record);
		}

		[TestMethod]
		public void Format_CustomDestinationFieldLayoutProvider_AllFieldsInquiriedAndRecordIsFormatted()
		{
			var destinationFieldLayoutProvider = MockRepository.GenerateMock<IDestinationFieldLayoutProvider>();
			var formatter = new FixedWidthTextRecordFormatter(destinationFieldLayoutProvider: destinationFieldLayoutProvider);
			var record = new MockRecord("text", 10, -1);

			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true)
				.Repeat.Once();
			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(10).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
				.Repeat.Once();
			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(18).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
				.Repeat.Once();
			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();
			
			var formattedRecord = formatter.Format(record);

			destinationFieldLayoutProvider.VerifyAllExpectations();
			
			Assert.AreEqual("text            10      -1", formattedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Format_DestinationFieldLayoutProviderThrowsException_ExceptionIsPropogated()
		{
			var destinationFieldLayoutProvider = MockRepository.GenerateMock<IDestinationFieldLayoutProvider>();
			var formatter = new FixedWidthTextRecordFormatter(destinationFieldLayoutProvider: destinationFieldLayoutProvider);
			var record = new MockRecord("text", 10, -1);

			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true)
				.Throw(new InternalTestFailureException());
			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(10).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
				.Throw(new InternalTestFailureException());
			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(18).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
				.Throw(new InternalTestFailureException());
			destinationFieldLayoutProvider.Expect(x => x.TryGetDestinationFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Throw(new InternalTestFailureException());
			
			var formattedRecord = formatter.Format(record);
		}

		[TestMethod]
		public void Format_CustomTextRecordFieldFormatter_CustomFieldFormatterUsedAndRecordIsFormatted()
		{
			var textRecordFieldFormatter = MockRepository.GenerateMock<ITextRecordFieldFormatter>();
			var formatter = new FixedWidthTextRecordFormatter(fieldFormatter: textRecordFieldFormatter);
			var record = new MockRecord("text", 10, -1);

			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal("text"), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)))).Return("text").Repeat.Once();
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(10), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)))).Return("10").Repeat.Once();
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(-1), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)))).Return("-1").Repeat.Once();
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)))).Repeat.Never();

			var formattedRecord = formatter.Format(record);

			textRecordFieldFormatter.VerifyAllExpectations();

			Assert.AreEqual("text            10      -1", formattedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Format_TextRecordFieldFormatterThrowsException_ExceptionIsPropogated()
		{
			var textRecordFieldFormatter = MockRepository.GenerateMock<ITextRecordFieldFormatter>();
			var formatter = new FixedWidthTextRecordFormatter(fieldFormatter: textRecordFieldFormatter);
			var record = new MockRecord("text", 10, -1);

			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal("text"), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)))).Throw(new InternalTestFailureException());
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(10), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)))).Throw(new InternalTestFailureException());
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(-1), Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)))).Throw(new InternalTestFailureException());
			textRecordFieldFormatter.Expect(x => x.Format(Arg<object>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)))).Repeat.Never();

			var formattedRecord = formatter.Format(record);
		}

		[TestMethod]
		public void Format_CustomFieldPaddingStrategy_CustomFieldPaddingStrategyUsedAndRecordIsFormatted()
		{
			var fieldPaddingStrategy = MockRepository.GenerateMock<ITextRecordFieldPaddingStrategy>();
			var formatter = new FixedWidthTextRecordFormatter(fieldPaddingStrategy: fieldPaddingStrategy);
			var record = new MockRecord("text", 10, -1);

			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)))).Return("text      ").Repeat.Once();
			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)))).Return("10      ").Repeat.Once();
			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)))).Return("-1      ").Repeat.Once();
			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)))).Repeat.Never();

			var formattedRecord = formatter.Format(record);

			fieldPaddingStrategy.VerifyAllExpectations();

			Assert.AreEqual("text      10      -1      ", formattedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Format_FieldPaddingStrategyThrowsException_ExceptionIsPropogated()
		{
			var fieldPaddingStrategy = MockRepository.GenerateMock<ITextRecordFieldPaddingStrategy>();
			var formatter = new FixedWidthTextRecordFormatter(fieldPaddingStrategy: fieldPaddingStrategy);
			var record = new MockRecord("text", 10, -1);

			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.StringField)))).Throw(new InternalTestFailureException());
			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.IntField)))).Throw(new InternalTestFailureException());
			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NullableIntField)))).Throw(new InternalTestFailureException());
			fieldPaddingStrategy.Expect(x => x.Pad(Arg<string>.Is.Anything, Arg<int>.Is.Anything, Arg<PropertyInfo>.Matches(y => y.Name == nameof(MockRecord.NotUsed)))).Repeat.Never();
			
			var formattedRecord = formatter.Format(record);
		}

		private class MockRecord
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

			[DestinationFieldLayout(0, 10)]
			public string StringField { get; set; }
			[DestinationFieldLayout(10, 8)]
			public int IntField { get; set; }
			[DestinationFieldLayout(18, 8)]
			public int? NullableIntField { get; set; }
			public string NotUsed { get; set; }
		}

		private class MockRecordWithMissingDestinationFieldLayout
		{
			public MockRecordWithMissingDestinationFieldLayout(string stringField, int intField, int? nullableIntField)
			{
				this.StringField = stringField;
				this.IntField = intField;
				this.NullableIntField = nullableIntField;
			}

			public MockRecordWithMissingDestinationFieldLayout()
			{
			}

			[DestinationFieldLayout(0, 10)]
			public string StringField { get; set; }
			[DestinationFieldLayout(10, 8)]
			public int IntField { get; set; }
			[DestinationFieldLayout(20, 8)]
			public int? NullableIntField { get; set; }
			public string NotUsed { get; set; }
		}
	}
}
