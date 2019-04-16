using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class RoundToAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_DoubleFieldWithMidwayValueWithToEvenRounding_ValueIsRoundedDown()
		{
			var record = new MockRecord() { DoubleField = 10.5d };
			var decimals = 0;
			var midpointRounding = MidpointRounding.ToEven;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var roundToAttribute = new RoundToAttribute(decimals, midpointRounding);

			roundToAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleFieldMidwayValueWithAwayFromZeroRounding_ValueIsRoundedUp()
		{
			var record = new MockRecord() { DoubleField = 10.5d };
			var decimals = 0;
			var midpointRounding = MidpointRounding.AwayFromZero;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var roundToAttribute = new RoundToAttribute(decimals, midpointRounding);

			roundToAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11d, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalFieldWithMidwayValueWithToEvenRounding_ValueIsRoundedDown()
		{
			var record = new MockRecord() { DecimalField = 10.5m };
			var decimals = 0;
			var midpointRounding = MidpointRounding.ToEven;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var roundToAttribute = new RoundToAttribute(decimals, midpointRounding);

			roundToAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalFieldMidwayValueWithAwayFromZeroRounding_ValueIsRoundedUp()
		{
			var record = new MockRecord() { DecimalField = 10.5m };
			var decimals = 0;
			var midpointRounding = MidpointRounding.AwayFromZero;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var roundToAttribute = new RoundToAttribute(decimals, midpointRounding);

			roundToAttribute.ApplyTransform(property, record);

			Assert.AreEqual(11m, record.DecimalField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ApplyTransform_InvalidType_ExceptionIsThrown()
		{
			var record = new MockRecord() { StringField = "Test" };
			var decimals = 0;
			var midpointRounding = MidpointRounding.AwayFromZero;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var roundToAttribute = new RoundToAttribute(decimals, midpointRounding);

			roundToAttribute.ApplyTransform(property, record);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithValue_ValueIsRounded()
		{
			var record = new MockRecord() { NullableDoubleField = 10.5d };
			var decimals = 0;
			var midpointRounding = MidpointRounding.ToEven;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var roundToAttribute = new RoundToAttribute(decimals, midpointRounding);

			roundToAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10d, record.NullableDoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleFieldWithNullValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { NullableDoubleField = null };
			var decimals = 0;
			var midpointRounding = MidpointRounding.ToEven;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var roundToAttribute = new RoundToAttribute(decimals, midpointRounding);

			roundToAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.NullableDoubleField);
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
