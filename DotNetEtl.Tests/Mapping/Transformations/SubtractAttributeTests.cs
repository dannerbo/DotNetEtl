using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class SubtractAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_DoubleFieldWithFractionalValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { DoubleField = 10.5d };
			var amount = 1.25d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);

			Assert.AreEqual(9.25d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalFieldWithFractionalValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { DecimalField = 10.5m };
			var amount = 1.25d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);

			Assert.AreEqual(9.25m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithFractionalValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { NullableDoubleField = 10.5d };
			var amount = 1.25d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);

			Assert.AreEqual(9.25d, record.NullableDoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDecimalFieldWithFractionalValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { NullableDecimalField = 10.5m };
			var amount = 1.25d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDecimalField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);

			Assert.AreEqual(9.25m, record.NullableDecimalField);
		}

		[TestMethod]
		public void ApplyTransform_IntFieldWithFractionalValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { IntField = 10 };
			var amount = 1.25d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);

			Assert.AreEqual(9, record.IntField);
		}

		[TestMethod]
		public void ApplyTransform_IntFieldWitWholeValue_ValueIsSubtracted()
		{
			var record = new MockRecord() { IntField = 10 };
			var amount = 1d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);

			Assert.AreEqual(9, record.IntField);
		}

		[TestMethod]
		[ExpectedException(typeof(RuntimeBinderException))]
		public void ApplyTransform_StringField_ValueIsSubtracted()
		{
			var record = new MockRecord() { StringField = "Test" };
			var amount = 1d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithNullValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { NullableDoubleField = null };
			var amount = 1.25d;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var subtractAttribute = new SubtractAttribute(amount);

			subtractAttribute.ApplyTransform(property, record);

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
			public string StringField { get; set; }
		}
	}
}
