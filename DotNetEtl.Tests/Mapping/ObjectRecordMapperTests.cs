using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Mapping.Tests
{
	[TestClass]
	public class ObjectRecordMapperTests
	{
		[TestMethod]
		public void TryMap_ValidSource_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceStringField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceDecimalField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableDateTimeField)).Dummy))
				.Return(true)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceStringField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDecimalField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceNullableIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceNullableDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();
			
			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			var couldMap = objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldNameProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(source.SourceIntField, record.IntField);
			Assert.AreEqual(source.SourceStringField, record.StringField);
			Assert.AreEqual(source.SourceDecimalField, record.DecimalField);
			Assert.AreEqual(source.SourceDateTimeField, record.DateTimeField);
			Assert.AreEqual(source.SourceNullableIntField, record.NullableIntField);
			Assert.AreEqual(source.SourceNullableDateTimeField, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_ValidSourceWithNullSourceFields_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = null,
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = null,
				SourceNullableDateTimeField = null
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceStringField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceDecimalField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableDateTimeField)).Dummy))
				.Return(true)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDecimalField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			var couldMap = objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldNameProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(source.SourceIntField, record.IntField);
			Assert.AreEqual(source.SourceStringField, record.StringField);
			Assert.AreEqual(source.SourceDecimalField, record.DecimalField);
			Assert.AreEqual(source.SourceDateTimeField, record.DateTimeField);
			Assert.AreEqual(source.SourceNullableIntField, record.NullableIntField);
			Assert.AreEqual(source.SourceNullableDateTimeField, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_ValidSourceWithDefaultDependencies_RecordIsMapped()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecordWithAttributes();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			
			var objectRecordMapper = new ObjectRecordMapper(recordFactory);

			var couldMap = objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(source.SourceIntField, record.IntField);
			Assert.AreEqual(source.SourceStringField, record.StringField);
			Assert.AreEqual(source.SourceDecimalField, record.DecimalField);
			Assert.AreEqual(source.SourceDateTimeField, record.DateTimeField);
			Assert.AreEqual(source.SourceNullableIntField, record.NullableIntField);
			Assert.AreEqual(source.SourceNullableDateTimeField, record.NullableDateTimeField);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_MissingField_FailureIsReturned()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceStringField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceDecimalField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out("NonExistantField").Dummy))
				.Return(true)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceStringField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDecimalField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceNullableIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Never();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")))).Return("NullableDateTimeField");

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			var couldMap = objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldNameProvider.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual(nameof(MockRecord.NullableDateTimeField), failures.Single().FieldName);
			Assert.AreEqual("Field is missing.", failures.Single().Message);
		}
		
		[TestMethod]
		public void TryMap_FieldCannotBeParsed_FailureIsReturned()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceStringField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceDecimalField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableIntField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableDateTimeField)).Dummy))
				.Return(true)
				.Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceStringField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDecimalField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceNullableIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field is invalid.").Dummy))
				.Return(false)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))), Arg<object>.Is.Equal(record))).Repeat.Never();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NotInSource))), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")))).Return("NullableDateTimeField");

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			var couldMap = objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldNameProvider.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual(nameof(MockRecord.NullableDateTimeField), failures.Single().FieldName);
			Assert.AreEqual("Field is invalid.", failures.Single().Message);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_RecordFactoryThrowsException_ExceptionIsPropogated()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Throw(new InternalTestFailureException());

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_SourceFieldNameProviderThrowsException_ExceptionIsPropogated()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<string>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldParserThrowsException_ExceptionIsPropogated()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceStringField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceDecimalField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableDateTimeField)).Dummy))
				.Return(true);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldTransformerThrowsException_ExceptionIsPropogated()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceStringField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceDecimalField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableDateTimeField)).Dummy))
				.Return(true);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceStringField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDecimalField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceNullableIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceNullableDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);

			fieldTransformer.Stub(x => x.ApplyTransforms(Arg<PropertyInfo>.Is.Anything, Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldDisplayNameProviderThrowsException_ExceptionIsPropogated()
		{
			var source = new MockSourceRecord()
			{
				SourceIntField = 1,
				SourceStringField = "text",
				SourceDecimalField = 2.00m,
				SourceDateTimeField = DateTime.Parse("2000-01-01"),
				SourceNullableIntField = 3,
				SourceNullableDateTimeField = DateTime.Parse("2000-01-02")
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceStringField)).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceDecimalField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableIntField)).Dummy))
				.Return(true);
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(nameof(MockSourceRecord.SourceNullableDateTimeField)).Dummy))
				.Return(true);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceStringField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DecimalField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDecimalField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.DateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceDateTimeField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(source.SourceNullableIntField).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.NullableDateTimeField))),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field is invalid.").Dummy))
				.Return(false);

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Throw(new InternalTestFailureException());

			var objectRecordMapper = new ObjectRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldNameProvider);

			objectRecordMapper.TryMap(source, out var mappedRecord, out var failures);
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
			[SourceFieldName("SourceIntField")]
			public int IntField { get; set; }
			[SourceFieldName("SourceStringField")]
			public string StringField { get; set; }
			[SourceFieldName("SourceDecimalField")]
			public decimal DecimalField { get; set; }
			[SourceFieldName("SourceDateTimeField")]
			public DateTime DateTimeField { get; set; }
			[SourceFieldName("SourceNullableIntField")]
			public int? NullableIntField { get; set; }
			[SourceFieldName("SourceNullableDateTimeField")]
			public DateTime? NullableDateTimeField { get; set; }
			public string NotInSource { get; set; }
		}

		private class MockSourceRecord
		{
			public int SourceIntField { get; set; }
			public string SourceStringField { get; set; }
			public decimal SourceDecimalField { get; set; }
			public DateTime SourceDateTimeField { get; set; }
			public int? SourceNullableIntField { get; set; }
			public DateTime? SourceNullableDateTimeField { get; set; }
			public string NotUsed { get; set; }
		}
	}
}
