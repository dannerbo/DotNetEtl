using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class CeilingAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_DecimalField1_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { DecimalField = 10m };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalField2_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { DecimalField = 10.1m };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalField3_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { DecimalField = 10.9m };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleField1_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { DoubleField = 10d };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleField2_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { DoubleField = 10.1d };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleField3_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { DoubleField = 10.9d };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11d, record.DoubleField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ApplyTransform_IntField_ExceptionIsThrown()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);
		}
		
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ApplyTransform_StringField_ExeptionThrown()
		{
			var record = new MockRecord() { StringField = "10" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);
		}

		[TestMethod]
		public void ApplyTransform_NullableDecimalField_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { NullableDecimalField = 10.1m };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDecimalField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11m, record.NullableDecimalField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleField_CeilingAppliedToProperty()
		{
			var record = new MockRecord() { NullableDoubleField = 10.1d };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var ceilingAttribute = new CeilingAttribute();

			ceilingAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11d, record.NullableDoubleField);
		}
		
		private class MockRecord
		{
			public int IntField { get; set; }
			public decimal DecimalField { get; set; }
			public double DoubleField { get; set; }
			public string StringField { get; set; }
			public int? NullableIntField { get; set; }
			public decimal? NullableDecimalField { get; set; }
			public double? NullableDoubleField { get; set; }
		}
	}
}
