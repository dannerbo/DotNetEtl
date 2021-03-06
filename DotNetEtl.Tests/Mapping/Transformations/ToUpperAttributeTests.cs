﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Transformations.Tests
{
	[TestClass]
	public class ToUpperAttributeTests
	{
		[TestMethod]
		public void ApplyTransform_StringFieldWithMixedCaseValue_TextIsLowered()
		{
			var record = new MockRecord() { StringField = "Test" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var toUpperAttribute = new ToUpperAttribute();

			toUpperAttribute.ApplyTransform(property, record);

			Assert.AreEqual("TEST", record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithEmptyValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = "" };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var toUpperAttribute = new ToUpperAttribute();

			toUpperAttribute.ApplyTransform(property, record);

			Assert.AreEqual(String.Empty, record.StringField);
		}

		[TestMethod]
		public void ApplyTransform_StringFieldWithNullValue_ValueIsUnchanged()
		{
			var record = new MockRecord() { StringField = null };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.StringField));
			var toUpperAttribute = new ToUpperAttribute();

			toUpperAttribute.ApplyTransform(property, record);

			Assert.IsNull(record.StringField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void ApplyTransform_IntField_ExceptionIsThrown()
		{
			var record = new MockRecord() { IntField = 10 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntField));
			var toUpperAttribute = new ToUpperAttribute();

			toUpperAttribute.ApplyTransform(property, record);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
		}
	}
}
