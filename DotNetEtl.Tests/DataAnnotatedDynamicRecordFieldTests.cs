using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataAnnotatedDynamicRecordFieldTests
	{
		[TestMethod]
		public void DataAnnotatedDynamicRecordField_DataTypeIsProvided_DataTypeAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				DataType = DataType.EmailAddress
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var dataTypeAttribute = (DataTypeAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is DataTypeAttribute);

			Assert.AreEqual(DataType.EmailAddress, dataTypeAttribute.DataType);
		}

		[TestMethod]
		public void GetCustomAttributes_EnumDataTypeIsProvided_EnumDataTypeAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(MockEnum),
				EnumDataType = typeof(MockEnum)
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var enumDataTypeAttribute = (EnumDataTypeAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is EnumDataTypeAttribute);

			Assert.AreEqual(typeof(MockEnum), enumDataTypeAttribute.EnumType);
		}

		[TestMethod]
		public void GetCustomAttributes_RangeDataTypeIsProvided_ZeroAttributesAreCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(int),
				RangeDataType = typeof(int)
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var customAttributes = property.GetCustomAttributes(true);
			
			Assert.AreEqual(0, customAttributes.Count());
		}

		[TestMethod]
		public void GetCustomAttributes_RangeMinimumIsProvided_ZeroAttributesAreCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(int),
				RangeMinimum = "1"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var customAttributes = property.GetCustomAttributes(true);

			Assert.AreEqual(0, customAttributes.Count());
		}

		[TestMethod]
		public void GetCustomAttributes_RangeMaximumIsProvided_ZeroAttributesAreCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(int),
				RangeMaximum = "10"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var customAttributes = property.GetCustomAttributes(true);

			Assert.AreEqual(0, customAttributes.Count());
		}
		
		[TestMethod]
		public void GetCustomAttributes_RangeDataTypeAndMinMaxValuesAreProvided_RangeAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(int),
				RangeDataType = typeof(int),
				RangeMinimum = "1",
				RangeMaximum = "10"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var rangeAttribute = (RangeAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is RangeAttribute);

			Assert.AreEqual(typeof(int), rangeAttribute.OperandType);
			Assert.AreEqual("1", rangeAttribute.Minimum);
			Assert.AreEqual("10", rangeAttribute.Maximum);
		}

		[TestMethod]
		public void GetCustomAttributes_RegularExpressionIsProvided_RegularExpressionAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				RegularExpression = "^.*$"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var regularExpressionAttribute = (RegularExpressionAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is RegularExpressionAttribute);

			Assert.AreEqual("^.*$", regularExpressionAttribute.Pattern);
		}

		[TestMethod]
		public void GetCustomAttributes_RequiredIsProvided_RequiredAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				Required = true
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var requiredAttribute = (RequiredAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is RequiredAttribute);
		}

		[TestMethod]
		public void GetCustomAttributes_MinLengthIsProvided_MinLengthAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				MinLength = 1
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var minLengthAttribute = (MinLengthAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is MinLengthAttribute);

			Assert.AreEqual(1, minLengthAttribute.Length);
		}

		[TestMethod]
		public void GetCustomAttributes_MaxLengthIsProvided_MaxLengthAttributeIsCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				MaxLength = 10
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var maxLengthAttribute = (MaxLengthAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is MaxLengthAttribute);

			Assert.AreEqual(10, maxLengthAttribute.Length);
		}

		[TestMethod]
		public void GetCustomAttributes_CustomValidatorTypeIsProvided_ZeroAttributesAreCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				CustomValidatorType = typeof(object)
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var customAttributes = property.GetCustomAttributes(true);

			Assert.AreEqual(0, customAttributes.Count());
		}

		[TestMethod]
		public void GetCustomAttributes_CustomValidatorMethodIsProvided_ZeroAttributesAreCreated()
		{
			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				CustomValidatorMethod = "CustomValidatorMethod"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var customAttributes = property.GetCustomAttributes(true);

			Assert.AreEqual(0, customAttributes.Count());
		}

		[TestMethod]
		public void GetCustomAttributes_CustomValidatorTypeAndMethodIsProvided_CustomValidatorAttributeIsCreated()
		{

			var fieldProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();
			var dynamicRecordFactory = new DynamicRecordFactory(fieldProvider);

			var dynamicRecordField = new DataAnnotatedDynamicRecordField()
			{
				Name = "Test",
				DotNetDataType = typeof(string),
				CustomValidatorType = typeof(object),
				CustomValidatorMethod = "CustomValidatorMethod"
			};

			var fields = new List<IDynamicRecordField>() { dynamicRecordField };

			fieldProvider.Stub(x => x.GetFields()).Return(fields);

			var record = dynamicRecordFactory.Create(null);

			var property = record.GetType().GetProperty(dynamicRecordField.Name);
			var customValidationAttribute = (CustomValidationAttribute)property
				.GetCustomAttributes(true)
				.Single(x => x is CustomValidationAttribute);

			Assert.AreEqual("CustomValidatorMethod", customValidationAttribute.Method);
		}

		private enum MockEnum
		{
		}
	}
}
