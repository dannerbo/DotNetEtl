using System;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Mapping.Tests
{
	[TestClass]
	public class DataRecordRecordMapperTests
	{
		[TestMethod]
		public void TryMap_ValidSource_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var field0 = "text";
			var field1 = 1;
			var field2 = 2;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(null).Dummy))
				.Return(false)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => !y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(0))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false).Repeat.Once();

			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(0))).Return(field0).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3).Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(field0),
					out Arg<object>.Out(field0).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(field1),
					out Arg<object>.Out(field1).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(field2),
					out Arg<object>.Out(field2).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(field3),
					out Arg<object>.Out(field3).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			var couldMap = dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			source.VerifyAllExpectations();
			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();
			sourceFieldNameProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(field0, record.Field0);
			Assert.AreEqual(field1, record.Field1);
			Assert.AreEqual(field2, record.Field2);
			Assert.AreEqual(field3, record.Field3);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_ValidSourceWithNullSourceField_AllDependenciesAreInvokedAsExpectedAndRecordIsMapped()
		{
			var field0 = "text";
			var field1 = 1;
			var field2 = (int?)null;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(0).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true)
				.Repeat.Once();
			sourceFieldOrdinalProvider.Expect(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false)
				.Repeat.Once();

			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(null).Dummy))
				.Return(false)
				.Repeat.Once();
			sourceFieldNameProvider.Expect(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => !y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(0))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false).Repeat.Once();

			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(0))).Return(field0).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3).Repeat.Once();

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(field0),
					out Arg<object>.Out(field0).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(field1),
					out Arg<object>.Out(field1).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(field2),
					out Arg<object>.Out(field2).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(field3),
					out Arg<object>.Out(field3).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")), Arg<object>.Is.Equal(record))).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")), Arg<object>.Is.Equal(record))).Repeat.Once();

			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Repeat.Never();

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			var couldMap = dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			source.VerifyAllExpectations();
			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
			sourceFieldOrdinalProvider.VerifyAllExpectations();
			sourceFieldNameProvider.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(field0, record.Field0);
			Assert.AreEqual(field1, record.Field1);
			Assert.IsNull(record.Field2);
			Assert.AreEqual(field3, record.Field3);
			Assert.IsNull(record.NotInSource);
		}

		[TestMethod]
		public void TryMap_ValidSourceWithDefaultDependencies_RecordIsMapped()
		{
			var field0 = "text";
			var field1 = 1;
			var field2 = 2;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecordWithAttributes();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			
			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			source.Expect(x => x.GetOrdinal(Arg<string>.Is.Equal("Field3"))).Return(3).Repeat.Once();

			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(0))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false).Repeat.Once();
			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false).Repeat.Once();

			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(0))).Return(field0).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2).Repeat.Once();
			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3).Repeat.Once();

			var dataRecordRecordMapper = new DataRecordRecordMapper(recordFactory);

			var couldMap = dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			source.VerifyAllExpectations();
			recordFactory.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(record, mappedRecord);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(field0, record.Field0);
			Assert.AreEqual(field1, record.Field1);
			Assert.AreEqual(field2, record.Field2);
			Assert.AreEqual(field3, record.Field3);
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

			var dataRecordRecordMapper = new DataRecordRecordMapper(recordFactory);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		public void TryMap_IDataRecordGetOrdinalThrowsExceptionForMissingField_FailureIsReturned()
		{
			var field0 = "text";
			var field1 = 1;
			var field2 = 2;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out("Field0").Dummy))
				.Return(true);

			source.Stub(x => x.GetOrdinal(Arg<string>.Is.Equal("Field0"))).Throw(new IndexOutOfRangeException());

			source.Expect(x => x.IsDBNull(Arg<int>.Is.Equal(0))).Return(false).Repeat.Never();
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false);

			source.Expect(x => x.GetValue(Arg<int>.Is.Equal(0))).Repeat.Never();
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3);

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(field0),
					out Arg<object>.Out(field0).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(field1),
					out Arg<object>.Out(field1).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(field2),
					out Arg<object>.Out(field2).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(field3),
					out Arg<object>.Out(field3).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")))).Return("Field0");

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")), Arg<object>.Is.Equal(record))).Repeat.Never();

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			var couldMap = dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);

			source.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual("Field0", failures.Single().FieldName);
			Assert.AreEqual("Field is missing or invalid.", failures.Single().Message);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_IDataRecordGetOrdinalThrowsRandomException_ExceptionIsPropogated()
		{
			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);
			
			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out("Field0").Dummy))
				.Return(true);

			source.Stub(x => x.GetOrdinal(Arg<string>.Is.Equal("Field0"))).Throw(new InternalTestFailureException());
			
			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		public void TryMap_IDataRecordGetValueThrowsExceptionForMissingField_FailureIsReturned()
		{
			var field0 = "text";
			var field1 = 1;
			var field2 = 2;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(0).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(0))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false);

			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(0))).Throw(new IndexOutOfRangeException());
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3);

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(field0),
					out Arg<object>.Out(field0).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(field1),
					out Arg<object>.Out(field1).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(field2),
					out Arg<object>.Out(field2).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(field3),
					out Arg<object>.Out(field3).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")))).Return("Field0");
			
			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			var couldMap = dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
			
			fieldParser.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual("Field0", failures.Single().FieldName);
			Assert.AreEqual("Field is missing or invalid.", failures.Single().Message);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_IDataRecordGetValueThrowsRandomException_ExceptionIsPropogated()
		{
			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(0).Dummy))
				.Return(true);

			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(0))).Throw(new InternalTestFailureException());

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_RecordFactoryThrowsException_ExceptionIsPropogated()
		{
			var source = MockRepository.GenerateMock<IDataRecord>();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Throw(new InternalTestFailureException());

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_SourceFieldOrdinalProviderThrowsException_ExceptionIsPropogated()
		{
			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<int>.Out(-1).Dummy))
				.Throw(new InternalTestFailureException());

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_SourceFieldNameProviderThrowsException_ExceptionIsPropogated()
		{
			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(0).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldParserThrowsException_ExceptionIsPropogated()
		{
			var field0 = "text";
			var field1 = 1;
			var field2 = 2;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(0).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(null).Dummy))
				.Return(false);

			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(0))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false);

			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(0))).Return(field0);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3);

			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldTransformerThrowsException_ExceptionIsPropogated()
		{
			var field0 = "text";
			var field1 = 1;
			var field2 = 2;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(0).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out(null).Dummy))
				.Return(false);

			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(0))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false);

			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(0))).Return(field0);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3);

			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(field0),
					out Arg<object>.Out(field0).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(field1),
					out Arg<object>.Out(field1).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(field2),
					out Arg<object>.Out(field2).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(field3),
					out Arg<object>.Out(field3).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Repeat.Never();

			fieldTransformer.Expect(x => x.ApplyTransforms(Arg<PropertyInfo>.Is.Anything, Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldDisplayNameProviderThrowsException_ExceptionIsPropogated()
		{
			var field1 = 1;
			var field2 = 2;
			var field3 = DateTime.Parse("2000-01-01");

			var source = MockRepository.GenerateMock<IDataRecord>();
			var record = new MockRecord();

			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();
			var sourceFieldOrdinalProvider = MockRepository.GenerateMock<ISourceFieldOrdinalProvider>();
			var sourceFieldNameProvider = MockRepository.GenerateMock<ISourceFieldNameProvider>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(1).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(2).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(3).Dummy))
				.Return(true);
			sourceFieldOrdinalProvider.Stub(x => x.TryGetSourceFieldOrdinal(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("NotInSource")),
					Arg<object>.Is.Equal(source),
					out Arg<int>.Out(-1).Dummy))
				.Return(false);

			sourceFieldNameProvider.Stub(x => x.TryGetSourceFieldName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")),
					Arg<object>.Is.Equal(source),
					out Arg<string>.Out("Field0").Dummy))
				.Return(true);

			source.Stub(x => x.GetOrdinal(Arg<string>.Is.Equal("Field0"))).Throw(new IndexOutOfRangeException());
			
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(1))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(2))).Return(false);
			source.Stub(x => x.IsDBNull(Arg<int>.Is.Equal(3))).Return(false);
			
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(1))).Return(field1);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(2))).Return(field2);
			source.Stub(x => x.GetValue(Arg<int>.Is.Equal(3))).Return(field3);
			
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field1")),
					Arg<object>.Is.Equal(field1),
					out Arg<object>.Out(field1).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field2")),
					Arg<object>.Is.Equal(field2),
					out Arg<object>.Out(field2).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field3")),
					Arg<object>.Is.Equal(field3),
					out Arg<object>.Out(field3).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true);

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Matches(y => y.Name.Equals("Field0")))).Throw(new InternalTestFailureException());

			var dataRecordRecordMapper = new DataRecordRecordMapper(
				recordFactory,
				fieldParser,
				fieldDisplayNameProvider,
				fieldTransformer,
				sourceFieldOrdinalProvider,
				sourceFieldNameProvider);

			dataRecordRecordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		private class MockRecord
		{
			public string Field0 { get; set; }
			public int Field1 { get; set; }
			public int? Field2 { get; set; }
			public DateTime Field3 { get; set; }
			public string NotInSource { get; set; }
		}

		private class MockRecordWithAttributes
		{
			[SourceFieldOrdinal(0)]
			public string Field0 { get; set; }
			[SourceFieldOrdinal(1)]
			public int Field1 { get; set; }
			[SourceFieldOrdinal(2)]
			public int? Field2 { get; set; }
			[SourceFieldName("Field3")]
			public DateTime Field3 { get; set; }
			public string NotInSource { get; set; }
		}
	}
}
