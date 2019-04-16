using System;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class DivideByAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_DoubleFieldWithAboveZeroValue_DivideByTransformApplied()
		{
			var record = new MockRecord() { DoubleField = 10d };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(5d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleFieldWithDivideByZero_InfinityIsSet()
		{
			var record = new MockRecord() { DoubleField = 10d };
			var amount = 0d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(Double.PositiveInfinity, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleFieldWithZeroValue_DivideByTransformApplied()
		{
			var record = new MockRecord() { DoubleField = 0d };
			var amount = 10d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithAboveZeroValue_DivideByTransformApplied()
		{
			var record = new MockRecord() { NullableDoubleField = 10d };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(5d, record.NullableDoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithNullValue_NullValueIsReturned()
		{
			var record = new MockRecord() { NullableDoubleField = null };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.NullableDoubleField);
		}
		
		[TestMethod]
		public void ApplyTransform_FloatFieldWithAboveZeroValue_DivideByTransformApplied()
		{
			var record = new MockRecord() { FloatField = 10f };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FloatField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(5f, record.FloatField);
		}

		[TestMethod]
		public void ApplyTransform_FloatFieldWithDivideByZero_InfinityIsSet()
		{
			var record = new MockRecord() { FloatField = 10f };
			var amount = 0d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FloatField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);
			
			Assert.AreEqual(Single.PositiveInfinity, record.FloatField);
		}

		[TestMethod]
		public void ApplyTransform_FloatFieldWithZeroValue_DivideByTransformApplied()
		{
			var record = new MockRecord() { FloatField = 0f };
			var amount = 10d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FloatField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0f, record.FloatField);
		}
		
		[TestMethod]
		public void ApplyTransform_DecimalFieldWithAboveZeroValue_DivideByTransformApplied()
		{
			var record = new MockRecord() { DecimalField = 10m };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(5m, record.DecimalField);
		}

		[TestMethod]
		[ExpectedException(typeof(DivideByZeroException))]
		public void ApplyTransform_DecimalFieldWithDivideByZero_ExceptionIsThrown()
		{
			var record = new MockRecord() { DecimalField = 10m };
			var amount = 0d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);
		}

		[TestMethod]
		public void ApplyTransform_DecimalFieldWithZeroValue_DivideByTransformApplied()
		{
			var record = new MockRecord() { DecimalField = 0m };
			var amount = 10d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_IntFieldWithFractionalDivideBy_DivideByTransformApplied()
		{
			var record = new MockRecord() { IntField = 10 };
			var amount = 2.5d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);

			Assert.AreEqual(4, record.IntField);
		}

		[TestMethod]
		[ExpectedException(typeof(RuntimeBinderException))]
		public void ApplyTransform_StringField_ExceptionIsThrown()
		{
			var record = new MockRecord() { StringField = "10" };
			var amount = 2d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var divideByAttribute = new DivideByAttribute(amount);

			divideByAttribute.ApplyTransform(property, record);
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
