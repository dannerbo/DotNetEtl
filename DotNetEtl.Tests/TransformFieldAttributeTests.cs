using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class TransformFieldAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_NullableTransformWithNonNullValue_FieldIsTransformed()
		{
			var record = new MockRecord() { FieldWithNullableTransformSupport = "Text" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithNullableTransformSupport));
			var transformFieldAttribute = property.GetCustomAttribute<TransformFieldAttribute>();

			transformFieldAttribute.ApplyTransform(property, record);

			Assert.AreEqual("NewValue", record.FieldWithNullableTransformSupport);
		}

		[TestMethod]
		public void ApplyTransform_NullableTransformWithNullValue_FieldIsTransformed()
		{
			var record = new MockRecord() { FieldWithNullableTransformSupport = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithNullableTransformSupport));
			var transformFieldAttribute = property.GetCustomAttribute<TransformFieldAttribute>();

			transformFieldAttribute.ApplyTransform(property, record);

			Assert.AreEqual("NewValue", record.FieldWithNullableTransformSupport);
		}

		[TestMethod]
		public void ApplyTransform_NotNullableTransformWithNonNullValue_FieldIsTransformed()
		{
			var record = new MockRecord() { FieldWithoutNullableTransformSupport = "Text" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutNullableTransformSupport));
			var transformFieldAttribute = property.GetCustomAttribute<TransformFieldAttribute>();

			transformFieldAttribute.ApplyTransform(property, record);

			Assert.AreEqual("NewValue", record.FieldWithoutNullableTransformSupport);
		}

		[TestMethod]
		public void ApplyTransform_NotNullableTransformWithNullValue_FieldIsNotTransformed()
		{
			var record = new MockRecord() { FieldWithoutNullableTransformSupport = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutNullableTransformSupport));
			var transformFieldAttribute = property.GetCustomAttribute<TransformFieldAttribute>();

			transformFieldAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.FieldWithoutNullableTransformSupport);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void ApplyTransform_ApplyTransformThrowsException_ExceptionIsPropogated()
		{
			var record = new MockRecord() { ErroringField = "Text" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.ErroringField));
			var transformFieldAttribute = property.GetCustomAttribute<TransformFieldAttribute>();

			transformFieldAttribute.ApplyTransform(property, record);
		}

		private class MockTransformFieldAttribute : TransformFieldAttribute
		{
			private object transformedValue;
			private bool canTransformNullValue;

			public MockTransformFieldAttribute(object transformedValue, bool canTransformNullValue)
			{
				this.transformedValue = transformedValue;
				this.canTransformNullValue = canTransformNullValue;
			}

			protected override bool CanTransformNullValue => this.canTransformNullValue;

			protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
			{
				return this.transformedValue;
			}
		}

		private class MockErroringTransformFieldAttribute : TransformFieldAttribute
		{
			protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
			{
				throw new InternalTestFailureException();
			}
		}

		private class MockRecord
		{
			[MockTransformField("NewValue", true)]
			public string FieldWithNullableTransformSupport { get; set; }
			[MockTransformField("NewValue", false)]
			public string FieldWithoutNullableTransformSupport { get; set; }
			[MockErroringTransformField]
			public string ErroringField { get; set; }
		}
	}
}
