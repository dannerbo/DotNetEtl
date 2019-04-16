using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordMapperTests
	{
		[TestMethod]
		public void TryMap_ValidRecord_RecordIsMapped()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(mappedRecord, record);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual("Text", ((MockRecord)mappedRecord).StringField);
			Assert.AreEqual(1, ((MockRecord)mappedRecord).IntField);
			Assert.AreEqual(2, ((MockRecord)mappedRecord).NullableIntField);
		}

		[TestMethod]
		public void TryMap_InvalidRecord_RecordIsNotMappedAndFailureIsReturned()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(false, null, "Field is not valid.");
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(mappedRecord, record);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual(nameof(MockRecord.StringField), failures.Single().FieldName);
			Assert.AreEqual("Field is not valid.", failures.Single().Message);
			Assert.IsNull(((MockRecord)mappedRecord).StringField);
			Assert.AreEqual(1, ((MockRecord)mappedRecord).IntField);
			Assert.AreEqual(2, ((MockRecord)mappedRecord).NullableIntField);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_TryReadSourceFieldThrowsException_ExceptionIsPropogated()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);

			var recordMapper = new MockRecordMapper(recordFactory);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				throw new InternalTestFailureException();
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		public void TryMap_ValidRecordWithFieldParser_RecordIsMapped()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal("Text"),
					out Arg<object>.Out("Text-Parsed").Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(1),
					out Arg<object>.Out(10).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(2),
					out Arg<object>.Out(20).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory, fieldParser: fieldParser);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(mappedRecord, record);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual("Text-Parsed", ((MockRecord)mappedRecord).StringField);
			Assert.AreEqual(10, ((MockRecord)mappedRecord).IntField);
			Assert.AreEqual(20, ((MockRecord)mappedRecord).NullableIntField);
		}

		[TestMethod]
		public void TryMap_InvalidRecordWithFieldParser_RecordIsMapped()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal("Text"),
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field could not be parsed.").Dummy))
				.Return(false)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(1),
					out Arg<object>.Out(10).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(2),
					out Arg<object>.Out(20).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory, fieldParser: fieldParser);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(mappedRecord, record);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual(nameof(MockRecord.StringField), failures.Single().FieldName);
			Assert.AreEqual("Field could not be parsed.", failures.Single().Message);
			Assert.IsNull(((MockRecord)mappedRecord).StringField);
			Assert.AreEqual(10, ((MockRecord)mappedRecord).IntField);
			Assert.AreEqual(20, ((MockRecord)mappedRecord).NullableIntField);
		}

		[TestMethod]
		public void TryMap_InvalidRecordWithFieldParserAndFieldDisplayNameProvider_RecordIsMapped()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal("Text"),
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field could not be parsed.").Dummy))
				.Return(false)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(1),
					out Arg<object>.Out(10).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(2),
					out Arg<object>.Out(20).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldDisplayNameProvider.Expect(x => x.GetFieldDisplayName(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField)))))
				.Return("StringField-NewName")
				.Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory, fieldParser: fieldParser, fieldDisplayNameProvider: fieldDisplayNameProvider);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldDisplayNameProvider.VerifyAllExpectations();

			Assert.IsFalse(couldMap);
			Assert.AreEqual(mappedRecord, record);
			Assert.AreEqual(1, failures.Count());
			Assert.AreEqual("StringField-NewName", failures.Single().FieldName);
			Assert.AreEqual("Field could not be parsed.", failures.Single().Message);
			Assert.IsNull(((MockRecord)mappedRecord).StringField);
			Assert.AreEqual(10, ((MockRecord)mappedRecord).IntField);
			Assert.AreEqual(20, ((MockRecord)mappedRecord).NullableIntField);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldParserThrowsException_ExceptionIsPropogated()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);
			fieldParser.Stub(x => x.TryParse(
					Arg<PropertyInfo>.Is.Anything,
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var recordMapper = new MockRecordMapper(recordFactory, fieldParser: fieldParser);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			recordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldDisplayNameProviderThrowsException_ExceptionIsPropogated()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal("Text"),
					out Arg<object>.Out(null).Dummy,
					out Arg<string>.Out("Field could not be parsed.").Dummy))
				.Return(false)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(1),
					out Arg<object>.Out(10).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(2),
					out Arg<object>.Out(20).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Throw(new InternalTestFailureException());

			var recordMapper = new MockRecordMapper(recordFactory, fieldParser: fieldParser, fieldDisplayNameProvider: fieldDisplayNameProvider);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			recordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		public void TryMap_ValidRecordWithFieldTransformer_RecordIsMappedAndTransformed()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(record)))
				.Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(record)))
				.Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(record)))
				.Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory, fieldTransformer: fieldTransformer);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(mappedRecord, record);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual("Text", ((MockRecord)mappedRecord).StringField);
			Assert.AreEqual(1, ((MockRecord)mappedRecord).IntField);
			Assert.AreEqual(2, ((MockRecord)mappedRecord).NullableIntField);
		}

		[TestMethod]
		public void TryMap_InvalidRecordWithFieldTransformer_TransformNotAppliedForInvalidField()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(record)))
				.Repeat.Never();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(record)))
				.Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(record)))
				.Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory, fieldTransformer: fieldTransformer);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(false, null, "Field is not valid.");
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryMap_FieldTransformerThrowsException_ExceptionIsPropogated()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();

			recordFactory.Stub(x => x.Create(Arg<object>.Is.Equal(source))).Return(record);
			fieldTransformer.Stub(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal(record)))
				.Throw(new InternalTestFailureException());

			var recordMapper = new MockRecordMapper(recordFactory, fieldTransformer: fieldTransformer);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			recordMapper.TryMap(source, out var mappedRecord, out var failures);
		}

		[TestMethod]
		public void TryMap_ValidRecordWithFieldParserAndFieldTransformer_RecordIsMappedAndTransformed()
		{
			var source = new object();
			var record = new MockRecord();
			var recordFactory = MockRepository.GenerateMock<IRecordFactory>();
			var fieldParser = MockRepository.GenerateMock<IFieldParser>();
			var fieldTransformer = MockRepository.GenerateMock<IFieldTransformer>();

			recordFactory.Expect(x => x.Create(Arg<object>.Is.Equal(source))).Return(record).Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Is.Equal("Text"),
					out Arg<object>.Out("Text-Parsed").Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Is.Equal(1),
					out Arg<object>.Out(10).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldParser.Expect(x => x.TryParse(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Is.Equal(2),
					out Arg<object>.Out(20).Dummy,
					out Arg<string>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.StringField))),
					Arg<object>.Matches(r => r.Equals(record) && ((MockRecord)r).StringField.Equals("Text-Parsed"))))
				.Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.IntField))),
					Arg<object>.Matches(r => r.Equals(record) && ((MockRecord)r).IntField.Equals(10))))
				.Repeat.Once();
			fieldTransformer.Expect(x => x.ApplyTransforms(
					Arg<PropertyInfo>.Matches(pi => pi.Name.Equals(nameof(MockRecord.NullableIntField))),
					Arg<object>.Matches(r => r.Equals(record) && ((MockRecord)r).NullableIntField.Equals(20))))
				.Repeat.Once();

			var recordMapper = new MockRecordMapper(recordFactory, fieldParser: fieldParser, fieldTransformer: fieldTransformer);

			recordMapper.TryReadSourceFieldFunc = new Func<PropertyInfo, object, Tuple<bool, object, string>>((pi, src) =>
			{
				switch (pi.Name)
				{
					case "StringField": return new Tuple<bool, object, string>(true, "Text", null);
					case "IntField": return new Tuple<bool, object, string>(true, 1, null);
					case "NullableIntField": return new Tuple<bool, object, string>(true, 2, null);
				}

				Assert.Fail();

				return null;
			});

			var couldMap = recordMapper.TryMap(source, out var mappedRecord, out var failures);

			recordFactory.VerifyAllExpectations();
			fieldParser.VerifyAllExpectations();
			fieldTransformer.VerifyAllExpectations();

			Assert.IsTrue(couldMap);
			Assert.AreEqual(mappedRecord, record);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual("Text-Parsed", ((MockRecord)mappedRecord).StringField);
			Assert.AreEqual(10, ((MockRecord)mappedRecord).IntField);
			Assert.AreEqual(20, ((MockRecord)mappedRecord).NullableIntField);
		}

		private class MockRecordMapper : RecordMapper
		{
			public MockRecordMapper(
				IRecordFactory recordFactory,
				IFieldParser fieldParser = null,
				IFieldDisplayNameProvider fieldDisplayNameProvider = null,
				IFieldTransformer fieldTransformer = null)
				: base(recordFactory, fieldParser, fieldDisplayNameProvider, fieldTransformer)
			{
			}

			public Func<PropertyInfo, object, Tuple<bool, object, string>> TryReadSourceFieldFunc { get; set; }

			protected override bool TryReadSourceField(PropertyInfo property, object parsedSourceRecord, out object fieldValue, out string failureMessage)
			{
				var tryReadSourceFieldFuncResult = this.TryReadSourceFieldFunc(property, parsedSourceRecord);

				fieldValue = tryReadSourceFieldFuncResult.Item2;
				failureMessage = tryReadSourceFieldFuncResult.Item3;

				return tryReadSourceFieldFuncResult.Item1;
			}
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
			public int? NullableIntField { get; set; }
		}
	}
}
