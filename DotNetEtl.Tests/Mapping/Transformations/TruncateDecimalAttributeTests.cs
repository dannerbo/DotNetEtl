using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class TruncateDecimalAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_DoubleFieldWithFractionalValue_ValueIsTruncated()
		{
			var record = new MockRecord() { DoubleField = 10.5d };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var truncateDecimalAttribute = new TruncateDecimalAttribute();

			truncateDecimalAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalFieldWithFractionalValue_ValueIsTruncated()
		{
			var record = new MockRecord() { DecimalField = 10.5m };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var truncateDecimalAttribute = new TruncateDecimalAttribute();

			truncateDecimalAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithFractionalValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { NullableDoubleField = 10.5d };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var truncateDecimalAttribute = new TruncateDecimalAttribute();

			truncateDecimalAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10d, record.NullableDoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDecimalFieldWithFractionalValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { NullableDecimalField = 10.5m };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDecimalField));
			var truncateDecimalAttribute = new TruncateDecimalAttribute();

			truncateDecimalAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10m, record.NullableDecimalField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ApplyTransform_IntField_ExceptionIsThrown()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var truncateDecimalAttribute = new TruncateDecimalAttribute();

			truncateDecimalAttribute.ApplyTransform(property, record);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithNullValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { NullableDoubleField = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var truncateDecimalAttribute = new TruncateDecimalAttribute();

			truncateDecimalAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.NullableDoubleField);
		}

		private class MockRecord
		{
			public double DoubleField { get; set; }
			public double? NullableDoubleField { get; set; }
			public float FloatField { get; set; }
			public decimal DecimalField { get; set; }
			public decimal? NullableDecimalField { get; set; }
			public int IntField { get; set; }
		}
	}
}
