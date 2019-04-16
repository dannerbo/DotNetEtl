using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Mapping.Tests
{
	[TestClass]
	public class DelimitedTextRecordMapperTests
	{
		[TestMethod]
		public void TryMap_ValidSource_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(intFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableDateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);
			
			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceRecordFieldCountProvider.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(intFieldValue, record.IntField);
			Assert.AreEqual(stringFieldValue, record.StringField);
			Assert.AreEqual(decimalFieldValue, record.DecimalField);
			Assert.AreEqual(dateTimeFieldValue, record.DateTimeField);
			Assert.AreEqual(nullableIntFieldValue, record.NullableIntField);
			Assert.AreEqual(nullableDateTimeFieldValue, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_ValidSourceWithPipeDelimiter_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue}|{stringFieldValue}|{decimalFieldValue}|{dateTimeFieldValue}|{nullableIntFieldValue}|{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = "|";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(intFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableDateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceRecordFieldCountProvider.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(intFieldValue, record.IntField);
			Assert.AreEqual(stringFieldValue, record.StringField);
			Assert.AreEqual(decimalFieldValue, record.DecimalField);
			Assert.AreEqual(dateTimeFieldValue, record.DateTimeField);
			Assert.AreEqual(nullableIntFieldValue, record.NullableIntField);
			Assert.AreEqual(nullableDateTimeFieldValue, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_ValidSourceWithNullSourceFields_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var intFieldValue = 1;
			var stringFieldValue = (string)null;
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = (int?)null;
			var nullableDateTimeFieldValue = (DateTime?)null;
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(intFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableDateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceRecordFieldCountProvider.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(intFieldValue, record.IntField);
			Assert.AreEqual(stringFieldValue, record.StringField);
			Assert.AreEqual(decimalFieldValue, record.DecimalField);
			Assert.AreEqual(dateTimeFieldValue, record.DateTimeField);
			Assert.AreEqual(nullableIntFieldValue, record.NullableIntField);
			Assert.AreEqual(nullableDateTimeFieldValue, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_ValidSourceWithDefaultDependencies_RecordIsMapped()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecordWithAttributes();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(recordFactory, delimiter);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(intFieldValue, record.IntField);
			Assert.AreEqual(stringFieldValue, record.StringField);
			Assert.AreEqual(decimalFieldValue, record.DecimalField);
			Assert.AreEqual(dateTimeFieldValue, record.DateTimeField);
			Assert.AreEqual(nullableIntFieldValue, record.NullableIntField);
			Assert.AreEqual(nullableDateTimeFieldValue, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryMap_WrongSourceType_ExceptionIsThrown()
		{
			var source = new object();
			var record = new MockRecordWithAttributes();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(recordFactory);

			delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		public void TryMap_MissingField_FailureIsReturned()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(intFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Never();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")))).Return("NullableDateTimeField");

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceRecordFieldCountProvider.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual("NullableDateTimeField", failures.Single().FieldName);
			Assert.AreEqual("Field is missing.", failures.Single().Message);
		}

		[TestMethod]
		public void TryMap_FieldCannotBeParsed_FailureIsReturned()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(intFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field is invalid.").Dummy))
				.Return(false)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Never();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")))).Return("NullableDateTimeField");

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceRecordFieldCountProvider.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual("NullableDateTimeField", failures.Single().FieldName);
			Assert.AreEqual("Field is invalid.", failures.Single().Message);
		}

		[TestMethod]
		public void TryMap_CorrectSourceFieldCountProvided_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(intFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableDateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			sourceRecordFieldCountProvider.Expect(x => x.TryGetSourceRecordFieldCount(
					Arg<Type>.Is.Equal(typeof(MockRecord)),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(6).Dummy))
				.Return(true)
				.Repeat.Once();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceRecordFieldCountProvider.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(intFieldValue, record.IntField);
			Assert.AreEqual(stringFieldValue, record.StringField);
			Assert.AreEqual(decimalFieldValue, record.DecimalField);
			Assert.AreEqual(dateTimeFieldValue, record.DateTimeField);
			Assert.AreEqual(nullableIntFieldValue, record.NullableIntField);
			Assert.AreEqual(nullableDateTimeFieldValue, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_IncorrectSourceFieldCountProvided_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Repeat.Never();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			sourceRecordFieldCountProvider.Expect(x => x.TryGetSourceRecordFieldCount(
					Arg<Type>.Is.Equal(typeof(MockRecord)),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(5).Dummy))
				.Return(true)
				.Repeat.Once();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Is.Anything, Arg<object>.Is.Anything)).Repeat.Never();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			var couldMap = delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceRecordFieldCountProvider.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(1, failures.Count());
			Assert.IsNull(failures.Single().FieldName);
			Assert.AreEqual("Expected 5 fields but encountered 6.", failures.Single().Message);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_RecordFactoryThrowsException_ExceptionIsPropogated()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Throw(new InternalTestFailureException());
			
			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_SourceFieldOrdinalProviderThrowsException_ExceptionIsPropogated()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Throw(new InternalTestFailureException());

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}
		
		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldParserThrowsException_ExceptionIsPropogated()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldTransformerThrowsException_ExceptionIsPropogated()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(intFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableDateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);

			fieldTransformer.Stub(x => x.ApplyTransforms(Arg<PropertyInfo>.Is.Anything, Arg<object>.Is.Equal(record))).Throw(new InternalTestFailureException());

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}
		
		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldDisplayNameProviderThrowsException_ExceptionIsPropogated()
		{
			var intFieldValue = 1;
			var stringFieldValue = "text";
			var decimalFieldValue = 2.00m;
			var dateTimeFieldValue = DateTime.Parse("2000-01-01");
			var nullableIntFieldValue = 3;
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = $"{intFieldValue},{stringFieldValue},{decimalFieldValue},{dateTimeFieldValue},{nullableIntFieldValue},{nullableDateTimeFieldValue}";
			var record = new MockRecord();
			var delimiter = ",";

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceRecordFieldCountProvider = MockRepository.GenerateMock<ISourceRecordFieldCountProvider>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(5).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field is invalid.").Dummy))
				.Return(false);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(stringFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(decimalFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(dateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableIntFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableDateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Throw(new InternalTestFailureException());

			var delimitedTextRecordMapper = new DelimitedTextRecordMapper(
				recordFactory,
				delimiter,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceRecordFieldCountProvider,
				sourceFieldOrdinalProvider);

			delimitedTextRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		private class MockRecord
		{
			public int IntField { get; set; }
			public string StringField { get; set; }
			public decimal DecimalField { get; set; }
			public DateTime DateTimeField { get; set; }
			public int? NullableIntField { get; set; }
			public DateTime? NullableDateTimeField { get; set; }
			public string NotInSource { get; set; }
		}

		private class MockRecordWithAttributes
		{
			[SourceFieldOrdinal(0)]
			public int IntField { get; set; }
			[SourceFieldOrdinal(1)]
			public string StringField { get; set; }
			[SourceFieldOrdinal(2)]
			public decimal DecimalField { get; set; }
			[SourceFieldOrdinal(3)]
			public DateTime DateTimeField { get; set; }
			[SourceFieldOrdinal(4)]
			public int? NullableIntField { get; set; }
			[SourceFieldOrdinal(5)]
			public DateTime? NullableDateTimeField { get; set; }
			public string NotInSource { get; set; }
		}
	}
}
