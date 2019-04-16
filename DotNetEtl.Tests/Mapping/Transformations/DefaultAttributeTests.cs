using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class DefaultAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_NullStringValue_DefaultValueSet()
		{
			var record = new MockRecord();
			var defaultValue = "Default";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual(defaultValue, record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_NonNullStringValue_OriginalValueRemains()
		{
			var record = new MockRecord() { StringField = "test" };
			var defaultValue = "Default";
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual("test", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_NonNullStringValueWithNullDefaultValue_OriginalValueRemains()
		{
			var record = new MockRecord() { StringField = "test" };
			var defaultValue = (string)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual("test", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_NullStringValueWithNullDefaultValue_NullValueReturned()
		{
			var record = new MockRecord() { StringField = null };
			var defaultValue = (string)null;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.StringField);
		}
		
		[TestMethod]
		public void ApplyTransform_ZeroDecimalValue_OriginalValueReimains()
		{
			var record = new MockRecord() { DecimalField = 0m };
			var defaultValue = 10m;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_NonZeroDecimalValue_OriginalValueReimains()
		{
			var record = new MockRecord() { DecimalField = 10m };
			var defaultValue = 20m;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10m, record.DecimalField);
		}

		[TestMethod]
		public void ApplyTransform_ZeroIntValue_OriginalValueReimains()
		{
			var record = new MockRecord() { IntField = 0 };
			var defaultValue = 10;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual(0, record.IntField);
		}

		[TestMethod]
		public void ApplyTransform_NonZeroIntValue_OriginalValueReimains()
		{
			var record = new MockRecord() { IntField = 10 };
			var defaultValue = 20;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10, record.IntField);
		}
		
		[TestMethod]
		public void ApplyTransform_NullZeroIntValue_DefaultValueSet()
		{
			var record = new MockRecord() { NullableIntField = null };
			var defaultValue = 10;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntField));
			var defaultAttribute = new DefaultAttribute(defaultValue);

			defaultAttribute.ApplyTransform(property, record);

			Assert.AreEqual(10, record.NullableIntField);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public decimal DecimalField { get; set; }
			public int IntField { get; set; }
			public int? NullableIntField { get; set; }
		}
	}
}
