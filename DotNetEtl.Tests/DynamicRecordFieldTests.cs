using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DynamicRecordFieldTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DynamicRecordField_NameIsNotProvided_ExceptionIsThrown()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = null,
				DotNetDataType = typeof(string),
				SourceFieldOrdinal = 1
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			dynamicRecordFactory.Create(null);
		}
		
		[TestMethod]
		[ExpectedException(typeof(NullReferenceException))]
		public void DynamicRecordField_DotNetDataTypeIsNotProvided_ExceptionIsThrown()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = "Field0",
				DotNetDataType = null,
				SourceFieldOrdinal = 1
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			dynamicRecordFactory.Create(null);
		}

		[TestMethod]
		public void DynamicRecordField_SourceFieldOrdinalIsProvided_SourceFieldOrdinalAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = "Field0",
				DotNetDataType = typeof(string),
				SourceFieldOrdinal = 1
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var sourceFieldOrdinalAttribute = (SourceFieldOrdinalAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldOrdinalAttribute);

			Assert.AreEqual(1, sourceFieldOrdinalAttribute.Ordinal);
		}
		
		[TestMethod]
		public void DynamicRecordField_SourceFieldLayoutIsProvided_SourceFieldLayoutAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = "Field0",
				DotNetDataType = typeof(string),
				SourceFieldStartIndex = 1,
				SourceFieldLength = 10
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var sourceFieldLayoutAttribute = (SourceFieldLayoutAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldLayoutAttribute);

			Assert.AreEqual(1, sourceFieldLayoutAttribute.StartIndex);
			Assert.AreEqual(10, sourceFieldLayoutAttribute.Length);
		}

		[TestMethod]
		public void DynamicRecordField_SourceFieldNameIsProvided_SourceFieldNameAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = "Field0",
				DotNetDataType = typeof(string),
				SourceFieldName = "Field0Name"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var sourceFieldNameAttribute = (SourceFieldNameAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is SourceFieldNameAttribute);

			Assert.AreEqual("Field0Name", sourceFieldNameAttribute.FieldName);
		}

		[TestMethod]
		public void DynamicRecordField_DestinationFieldOrdinalIsProvided_DestinationFieldOrdinalAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = "Field0",
				DotNetDataType = typeof(string),
				DestinationFieldOrdinal = 1
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var DestinationFieldOrdinalAttribute = (DestinationFieldOrdinalAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is DestinationFieldOrdinalAttribute);

			Assert.AreEqual(1, DestinationFieldOrdinalAttribute.Ordinal);
		}

		[TestMethod]
		public void DynamicRecordField_DestinationFieldLayoutIsProvided_DestinationFieldLayoutAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = "Field0",
				DotNetDataType = typeof(string),
				DestinationFieldStartIndex = 1,
				DestinationFieldLength = 10
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var DestinationFieldLayoutAttribute = (DestinationFieldLayoutAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is DestinationFieldLayoutAttribute);

			Assert.AreEqual(1, DestinationFieldLayoutAttribute.StartIndex);
			Assert.AreEqual(10, DestinationFieldLayoutAttribute.Length);
		}

		[TestMethod]
		public void DynamicRecordField_DestinationFieldNameIsProvided_DestinationFieldNameAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DynamicRecordField()
			{
				Name = "Field0",
				DotNetDataType = typeof(string),
				DestinationFieldName = "Field0Name"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var DestinationFieldNameAttribute = (DestinationFieldNameAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is DestinationFieldNameAttribute);

			Assert.AreEqual("Field0Name", DestinationFieldNameAttribute.FieldName);
		}
	}
}
