using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Mapping.Tests
{
	[TestClass]
	public class FixedWidthBinaryRecordMapperTests
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
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(14).Dummy,
					out Arg<int>.Out(16).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(30).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(38).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(42).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
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

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			var couldMap = fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldLayoutProvider.VerifyAllExpectations();

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
			var source = new byte[]
			{
				1, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(14).Dummy,
					out Arg<int>.Out(16).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(30).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(38).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(42).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
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
				.Return(false)
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
				.Return(false)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(nullableDateTimeFieldValue).Dummy,
					out Arg<string>.Out(null).Dummy))
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
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			var couldMap = fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldLayoutProvider.VerifyAllExpectations();

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
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};
			var record = new MockRecordWithAttributes();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			
			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(recordFactory);

			var couldMap = fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);

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

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(recordFactory);

			fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void TryMap_SourceIsInvalidLength_ExceptionIsThrown()
		{
			var nullableDateTimeFieldValue = DateTime.Parse("2000-01-02");
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(14).Dummy,
					out Arg<int>.Out(16).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(30).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(38).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(42).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);
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
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(14).Dummy,
					out Arg<int>.Out(16).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(30).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(38).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldLayoutProvider.Expect(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(42).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true)
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

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			var couldMap = fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldLayoutProvider.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual("NullableDateTimeField", failures.Single().FieldName);
			Assert.AreEqual("Field is invalid.", failures.Single().Message);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_RecordFactoryThrowsException_ExceptionIsPropogated()
		{
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());
			
			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_SourceFieldLayoutProviderThrowsException_ExceptionIsPropogated()
		{
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(0).Dummy))
				.Throw(new InternalTestFailureException());

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldParserThrowsException_ExceptionIsPropogated()
		{
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(14).Dummy,
					out Arg<int>.Out(16).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(30).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(38).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(42).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);
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
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(14).Dummy,
					out Arg<int>.Out(16).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(30).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(38).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(42).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);

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

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Is.Anything, Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);
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
			var source = new byte[]
			{
				1, 0, 0, 0,
				116, 101, 120, 116, 0, 0, 0, 0, 0, 0,
				200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,
				0, 64, 228, 71, 2, 34, 193, 8,
				3, 0, 0, 0,
				0, 0, 78, 114, 203, 34, 193, 8
			};
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldLayoutProvider = MockRepository.GenerateMock<ISourceFieldLayoutProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("IntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(0).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("StringField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(4).Dummy,
					out Arg<int>.Out(10).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DecimalField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(14).Dummy,
					out Arg<int>.Out(16).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("DateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(30).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableIntField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(38).Dummy,
					out Arg<int>.Out(4).Dummy))
				.Return(true);
			sourceFieldLayoutProvider.Stub(x => x.TryGetSourceFieldLayout(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NullableDateTimeField")),
					Arg<object>.Is.Anything,
					out Arg<int>.Out(42).Dummy,
					out Arg<int>.Out(8).Dummy))
				.Return(true);

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
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field is invalid.").Dummy))
				.Return(false);

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Throw(new InternalTestFailureException());

			var fixedWidthBinaryRecordMapper = new FixedWidthBinaryRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldLayoutProvider);

			fixedWidthBinaryRecordMapper.TryMap(source, out var mappedRecord, out var failures);
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
			[SourceFieldLayout(0, 4)]
			public int IntField { get; set; }
			[SourceFieldLayout(4, 10)]
			public string StringField { get; set; }
			[SourceFieldLayout(14, 16)]
			public decimal DecimalField { get; set; }
			[SourceFieldLayout(30, 8)]
			public DateTime DateTimeField { get; set; }
			[SourceFieldLayout(38, 4)]
			public int? NullableIntField { get; set; }
			[SourceFieldLayout(42, 8)]
			public DateTime? NullableDateTimeField { get; set; }
			public string NotInSource { get; set; }
		}
	}
}
