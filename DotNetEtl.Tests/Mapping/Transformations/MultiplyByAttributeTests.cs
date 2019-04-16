using System;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class MultiplyByAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_DoubleFieldWithAboveZeroValue_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { DoubleField = 10d };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleFieldWithMultiplyByZero_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { DoubleField = 10d };
			var amount = 0d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleFieldWithZeroValue_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { DoubleField = 0d };
			var amount = 10d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithAboveZeroValue_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { NullableDoubleField = 10d };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20d, record.NullableDoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithNullValue_NullValueIsReturned()
		{
			var record = new MockRecord() { NullableDoubleField = null };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.NullableDoubleField);
		}
		
		[TestMethod]
		public void ApplyTransform_FloatFieldWithAboveZeroValue_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { FloatField = 10f };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FloatField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20f, record.FloatField);
		}

		[TestMethod]
		public void ApplyTransform_FloatFieldWithMultiplyByZero_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { FloatField = 10f };
			var amount = 0d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FloatField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);
			
			Assert.AreEqual(0f, record.FloatField);
		}

		[TestMethod]
		public void ApplyTransform_FloatFieldWithZeroValue_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { FloatField = 0f };
			var amount = 10d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FloatField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0f, record.FloatField);
		}
		
		[TestMethod]
		public void ApplyTransform_DecimalFieldWithAboveZeroValue_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { DecimalField = 10m };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalFieldWithMultiplyByZero_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { DecimalField = 10m };
			var amount = 0d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalFieldWithZeroValue_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { DecimalField = 0m };
			var amount = 10d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_IntFieldWithFractionalMultiplyBy_MultiplyByTransformApplied()
		{
			var record = new MockRecord() { IntField = 10 };
			var amount = 2.5d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(25, record.IntField);
		}

		[TestMethod]
		[ExpectedException(typeof(RuntimeBinderException))]
		public void ApplyTransform_StringField_ExceptionIsThrown()
		{
			var record = new MockRecord() { StringField = "10" };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var multiplyByAttribute = new MultiplyByAttribute(amount);

			multiplyByAttribute.ApplyTransform(property, record);
		}
		
		private class MockRecord
		{
			public double DoubleField { get; set; }
			public double? NullableDoubleField { get; set; }
			public float FloatField { get; set; }
			public decimal DecimalField { get; set; }
			public int IntField { get; set; }
			public string StringField { get; set; }
		}
	}
}
