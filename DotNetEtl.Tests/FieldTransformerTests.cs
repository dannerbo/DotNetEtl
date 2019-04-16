using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class FieldTransformerTests
	{
		[TestMethod]
		public void ApplyTransforms_FieldWithTransformFieldAttributeAndNotNull_FieldIsTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithTransformFieldAttribute));
			var record = new MockRecord()
			{
				FieldWithTransformFieldAttribute = "Test"
			};
			
			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.AreEqual("Test-Test", record.FieldWithTransformFieldAttribute);
		}

		[TestMethod]
		public void ApplyTransforms_FieldWithTransformFieldAttributeAndIsNull_FieldIsNotTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithTransformFieldAttribute));
			var record = new MockRecord()
			{
				FieldWithTransformFieldAttribute = null
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.IsNull(record.FieldWithTransformFieldAttribute);
		}

		[TestMethod]
		public void ApplyTransforms_FieldWithMultipleTransformFieldAttributesAndIsNotNull_FieldIsTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithMultipleTransformFieldAttributes));
			var record = new MockRecord()
			{
				FieldWithMultipleTransformFieldAttributes = "Test"
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.AreEqual("Test-Test1-Test2", record.FieldWithMultipleTransformFieldAttributes);
		}
		
		[TestMethod]
		public void ApplyTransforms_FieldWithMultipleTransformFieldAttributesAndIsNull_FieldIsNotTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithMultipleTransformFieldAttributes));
			var record = new MockRecord()
			{
				FieldWithMultipleTransformFieldAttributes = null
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.IsNull(record.FieldWithMultipleTransformFieldAttributes);
		}

		[TestMethod]
		public void ApplyTransforms_FieldWithNullableTransformFieldAttributeAndNotNull_FieldIsTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithNullableTransformFieldAttribute));
			var record = new MockRecord()
			{
				FieldWithNullableTransformFieldAttribute = "Test"
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.AreEqual("Test-Test", record.FieldWithNullableTransformFieldAttribute);
		}

		[TestMethod]
		public void ApplyTransforms_FieldWithNullableTransformFieldAttributeAndIsNull_FieldIsTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithNullableTransformFieldAttribute));
			var record = new MockRecord()
			{
				FieldWithNullableTransformFieldAttribute = null
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.AreEqual("-Test", record.FieldWithNullableTransformFieldAttribute);
		}

		[TestMethod]
		public void ApplyTransforms_FieldWithoutTransformFieldAttributeAndNotNull_FieldIsNotTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutTransformFieldAttribute));
			var record = new MockRecord()
			{
				FieldWithoutTransformFieldAttribute = "Test"
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.AreEqual("Test", record.FieldWithoutTransformFieldAttribute);
		}

		[TestMethod]
		public void ApplyTransforms_FieldWithoutTransformFieldAttributeAndIsNull_FieldIsNotTransformed()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutTransformFieldAttribute));
			var record = new MockRecord()
			{
				FieldWithoutTransformFieldAttribute = null
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);

			Assert.IsNull(record.FieldWithoutTransformFieldAttribute);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void ApplyTransforms_TransformFieldAttributeApplyTransformsThrowsException_ExceptionIsPropogated()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.ExceptionThrowingField));
			var record = new MockRecord()
			{
				ExceptionThrowingField = "Test"
			};

			var fieldTransformer = new FieldTransformer();

			fieldTransformer.ApplyTransforms(property, record);
		}

		private class MockRecord
		{
			[MockConcatTransformField("-Test")]
			public string FieldWithTransformFieldAttribute { get; set; }
			[MockConcatTransformField("-Test1")]
			[MockConcatTransformField("-Test2")]
			public string FieldWithMultipleTransformFieldAttributes { get; set; }
			[MockConcatTransformField("-Test", true)]
			public string FieldWithNullableTransformFieldAttribute { get; set; }
			public string FieldWithoutTransformFieldAttribute { get; set; }
			[ExceptionThrowingTransformField]
			public string ExceptionThrowingField { get; set; }
		}

		private class MockConcatTransformFieldAttribute : TransformFieldAttribute
		{
			private string valueToAppend;
			private bool canTransformNullValue;

			public MockConcatTransformFieldAttribute(string valueToAppend, bool canTransformNullValue = false)
			{
				this.valueToAppend = valueToAppend;
				this.canTransformNullValue = canTransformNullValue;
			}

			protected override bool CanTransformNullValue => this.canTransformNullValue;

			protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
			{
				return (string)fieldValue + valueToAppend;
			}
		}

		private class ExceptionThrowingTransformFieldAttribute : TransformFieldAttribute
		{
			protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
			{
				throw new InternalTestFailureException();
			}
		}
	}
}
