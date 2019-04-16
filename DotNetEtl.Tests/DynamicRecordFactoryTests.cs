using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DynamicRecordFactoryTests
	{
		[TestMethod]
		public void Create_OneFieldWithNoAttributes_RecordIsCreatedWithExpectedField()
		{
			var source = (object)null;

			var sourceFieldOrdinalAttributeConstructor = typeof(SourceFieldOrdinalAttribute).GetConstructor(new Type[] { typeof(int) });

			var stringFieldAttributes = new List<CustomAttributeBuilder>();
			var stringField = MockRepository.GenerateMock<IDynamicRecordField>();

			stringField.Expect(x => x.Name).Return("StringField");
			stringField.Expect(x => x.DotNetDataType).Return(typeof(string));
			stringField.Expect(x => x.GetCustomAttributes()).Return(stringFieldAttributes);

			var fields = new List<IDynamicRecordField>() { stringField };

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			fieldProvider.Expect(x => x.GetFields()).Return(fields).Repeat.Once();

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var record = dynamicRecordFactory.Create(source);

			var stringFieldProperty = record.GetType().GetProperty("StringField");

			Assert.IsNotNull(stringFieldProperty);

			fieldProvider.VerifyAllExpectations();
			stringField.VerifyAllExpectations();
		}

		[TestMethod]
		public void Create_OneFieldWithOneAttribute_RecordIsCreatedWithExpectedFieldAndAttribute()
		{
			var source = (object)null;

			var sourceFieldOrdinalAttributeConstructor = typeof(SourceFieldOrdinalAttribute).GetConstructor(new Type[] { typeof(int) });

			var stringFieldAttributes = new List<CustomAttributeBuilder>()
			{
				new CustomAttributeBuilder(sourceFieldOrdinalAttributeConstructor, new object[] { 1 })
			};

			var stringField = MockRepository.GenerateMock<IDynamicRecordField>();

			stringField.Expect(x => x.Name).Return("StringField");
			stringField.Expect(x => x.DotNetDataType).Return(typeof(string));
			stringField.Expect(x => x.GetCustomAttributes()).Return(stringFieldAttributes);

			var fields = new List<IDynamicRecordField>() { stringField };

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			fieldProvider.Expect(x => x.GetFields()).Return(fields).Repeat.Once();

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);
			
			var record = dynamicRecordFactory.Create(source);

			var stringFieldProperty = record.GetType().GetProperty("StringField");
			var stringFieldSourceFieldOrdinalAttribute = (SourceFieldOrdinalAttribute)stringFieldProperty
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldOrdinalAttribute);
			
			Assert.AreEqual(1, stringFieldSourceFieldOrdinalAttribute.Ordinal);

			fieldProvider.VerifyAllExpectations();
			stringField.VerifyAllExpectations();
		}

		[TestMethod]
		public void Create_OneFieldWithTwoAttributes_RecordIsCreatedWithExpectedFieldAndAttributes()
		{
			var source = (object)null;

			var sourceFieldOrdinalAttributeConstructor = typeof(SourceFieldOrdinalAttribute).GetConstructor(new Type[] { typeof(int) });
			var destinationFieldNameAttributeConstructor = typeof(DestinationFieldNameAttribute).GetConstructor(new Type[] { typeof(string) });

			var stringFieldAttributes = new List<CustomAttributeBuilder>()
			{
				new CustomAttributeBuilder(sourceFieldOrdinalAttributeConstructor, new object[] { 1 }),
				new CustomAttributeBuilder(destinationFieldNameAttributeConstructor, new object[] { "StringFieldX" })
			};

			var stringField = MockRepository.GenerateMock<IDynamicRecordField>();

			stringField.Expect(x => x.Name).Return("StringField");
			stringField.Expect(x => x.DotNetDataType).Return(typeof(string));
			stringField.Expect(x => x.GetCustomAttributes()).Return(stringFieldAttributes);

			var fields = new List<IDynamicRecordField>() { stringField };

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			fieldProvider.Expect(x => x.GetFields()).Return(fields).Repeat.Once();

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var record = dynamicRecordFactory.Create(source);

			var stringFieldProperty = record.GetType().GetProperty("StringField");
			var stringFieldSourceFieldOrdinalAttribute = (SourceFieldOrdinalAttribute)stringFieldProperty
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldOrdinalAttribute);
			var stringFieldDestinationFieldNameAttribute = (DestinationFieldNameAttribute)stringFieldProperty
				.GetCustomAttributes(true)
				.Single(x => x is DestinationFieldNameAttribute);

			Assert.AreEqual(1, stringFieldSourceFieldOrdinalAttribute.Ordinal);
			Assert.AreEqual("StringFieldX", stringFieldDestinationFieldNameAttribute.FieldName);

			fieldProvider.VerifyAllExpectations();
			stringField.VerifyAllExpectations();
		}

		[TestMethod]
		public void Create_MultipleFieldsWithAttributes_RecordIsCreatedWithExpectedFieldsAndAttributes()
		{
			var source = (object)null;

			var sourceFieldOrdinalAttributeConstructor = typeof(SourceFieldOrdinalAttribute).GetConstructor(new Type[] { typeof(int) });

			var stringFieldAttributes = new List<CustomAttributeBuilder>()
			{
				new CustomAttributeBuilder(sourceFieldOrdinalAttributeConstructor, new object[] { 1 })
			};

			var stringField = MockRepository.GenerateMock<IDynamicRecordField>();

			stringField.Expect(x => x.Name).Return("StringField");
			stringField.Expect(x => x.DotNetDataType).Return(typeof(string));
			stringField.Expect(x => x.GetCustomAttributes()).Return(stringFieldAttributes);

			var intFieldAttributes = new List<CustomAttributeBuilder>()
			{
				new CustomAttributeBuilder(sourceFieldOrdinalAttributeConstructor, new object[] { 2 })
			};

			var intField = MockRepository.GenerateMock<IDynamicRecordField>();

			intField.Expect(x => x.Name).Return("IntField");
			intField.Expect(x => x.DotNetDataType).Return(typeof(int));
			intField.Expect(x => x.GetCustomAttributes()).Return(intFieldAttributes);

			var fields = new List<IDynamicRecordField>() { stringField, intField };

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			fieldProvider.Expect(x => x.GetFields()).Return(fields).Repeat.Once();

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var record = dynamicRecordFactory.Create(source);

			var stringFieldProperty = record.GetType().GetProperty("StringField");
			var intFieldProperty = record.GetType().GetProperty("IntField");
			var stringFieldSourceFieldOrdinalAttribute = (SourceFieldOrdinalAttribute)stringFieldProperty
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldOrdinalAttribute);
			var intFieldSourceFieldOrdinalAttribute = (SourceFieldOrdinalAttribute)intFieldProperty
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldOrdinalAttribute);

			Assert.AreEqual(1, stringFieldSourceFieldOrdinalAttribute.Ordinal);
			Assert.AreEqual(2, intFieldSourceFieldOrdinalAttribute.Ordinal);

			fieldProvider.VerifyAllExpectations();
			stringField.VerifyAllExpectations();
		}

		[TestMethod]
		public void Create_OneFieldWithOneAttributeWithRecordTypeProvider_RecordIsCreatedWithExpectedFieldAndAttribute()
		{
			var source = new object();
			var recordType = "RecordType";

			var sourceFieldOrdinalAttributeConstructor = typeof(SourceFieldOrdinalAttribute).GetConstructor(new Type[] { typeof(int) });

			var stringFieldAttributes = new List<CustomAttributeBuilder>()
			{
				new CustomAttributeBuilder(sourceFieldOrdinalAttributeConstructor, new object[] { 1 })
			};

			var stringField = MockRepository.GenerateMock<IDynamicRecordField>();

			stringField.Expect(x => x.Name).Return("StringField");
			stringField.Expect(x => x.DotNetDataType).Return(typeof(string));
			stringField.Expect(x => x.GetCustomAttributes()).Return(stringFieldAttributes);

			var fields = new List<IDynamicRecordField>() { stringField };

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var recordTypeProvider = MockRepository.GenerateMock<IRecordTypeProvider>();

			recordTypeProvider.Expect(x => x.GetRecordType(Arg<object>.Is.Equal(source))).Return(recordType).Repeat.Once();
			fieldProvider.Expect(x => x.GetFields(Arg<object>.Is.Equal(recordType))).Return(fields).Repeat.Once();

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider, recordTypeProvider);

			var record = dynamicRecordFactory.Create(source);

			var stringFieldProperty = record.GetType().GetProperty("StringField");
			var stringFieldSourceFieldOrdinalAttribute = (SourceFieldOrdinalAttribute)stringFieldProperty
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldOrdinalAttribute);

			Assert.AreEqual(1, stringFieldSourceFieldOrdinalAttribute.Ordinal);

			recordTypeProvider.VerifyAllExpectations();
			fieldProvider.VerifyAllExpectations();
			stringField.VerifyAllExpectations();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_RecordTypeProviderThrowsException_ExceptionIsPropogated()
		{
			var source = new object();

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var recordTypeProvider = MockRepository.GenerateMock<IRecordTypeProvider>();

			recordTypeProvider.Stub(x => x.GetRecordType(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider, recordTypeProvider);

			dynamicRecordFactory.Create(source);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_DynamicRecordFieldProviderThrowsException_ExceptionIsPropogated()
		{
			var source = (object)null;

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			fieldProvider.Stub(x => x.GetFields()).Throw(new InternalTestFailureException());

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			dynamicRecordFactory.Create(source);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Create_FieldProviderReturnsNull_ExceptionIsThrown()
		{
			var source = (object)null;
			var fields = (IEnumerable<IDynamicRecordField>)null;

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			fieldProvider.Expect(x => x.GetFields()).Return(fields).Repeat.Once();

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			try
			{
				dynamicRecordFactory.Create(source);
			}
			catch
			{
				fieldProvider.VerifyAllExpectations();

				throw;
			}

			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Create_FieldProviderReturnsEmptyList_ExceptionIsThrown()
		{
			var source = (object)null;
			var fields = new List<IDynamicRecordField>();

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			fieldProvider.Expect(x => x.GetFields()).Return(fields).Repeat.Once();

			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			try
			{
				dynamicRecordFactory.Create(source);
			}
			catch
			{
				fieldProvider.VerifyAllExpectations();

				throw;
			}

			Assert.Fail();
		}
	}
}
