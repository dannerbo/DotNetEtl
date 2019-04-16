using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class AddAttributeTests
	{
		[TestMethod]
		public void Constructor_AmountProvided_ProvidedAmountSet()
		{
			var amount = 10;
			var addAttribute = new AddAttribute(amount);

			Assert.AreEqual(amount, addAttribute.Amount);
		}

		[TestMethod]
		public void ApplyTransform_IntField_AmountAddedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20, record.IntField);
		}

		[TestMethod]
		public void ApplyTransform_DecimalField_AmountAddedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { DecimalField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_DoubleField_AmountAddedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { DoubleField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20, record.DoubleField);
		}

		[TestMethod]
		public void ApplyTransform_StringField_AmountAppendedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { StringField = "10" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.AreEqual("1010", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_NullableIntField_AmountAddedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { NullableIntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20, record.NullableIntField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDecimalField_AmountAddedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { NullableDecimalField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDecimalField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20, record.NullableDecimalField);
		}

		[TestMethod]
		public void ApplyTransform_NullableDoubleField_AmountAddedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { NullableDoubleField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.AreEqual(20, record.NullableDoubleField);
		}

		[TestMethod]
		public void ApplyTransform_NullValueWithNullableDoubleField_AmountAddedToProperty()
		{
			var amount = 10;
			var record = new MockRecord() { NullableDoubleField = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDoubleField));
			var addAttribute = new AddAttribute(amount);

			addAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.NullableDoubleField);
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
