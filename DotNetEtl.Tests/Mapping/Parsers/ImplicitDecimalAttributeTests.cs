using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Parsers.Tests
{
	[TestClass]
	public class ImplicitDecimalAttributeTests
	{
		[TestMethod]
		public void TryParse_ValidStringValueAndDecimalTypeTarget_CouldParse()
		{
			var value = "10050";
			var decimals = 2;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100.5m, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_ValidStringValueAndDoubleTypeTarget_CouldParse()
		{
			var value = "10050";
			var decimals = 2;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DoubleField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100.5d, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}
		
		[TestMethod]
		public void TryParse_ValidStringValueAndFloatTypeTarget_CouldParse()
		{
			var value = "10050";
			var decimals = 2;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FloatField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100.5f, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_ValidStringValueAndNullableDecimalTypeTarget_CouldParse()
		{
			var value = "10050";
			var decimals = 2;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableDecimalField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100.5m, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidStringValue_CouldNotParse()
		{
			var value = "invalid";
			var decimals = 2;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryParse_NonStringValue_ExceptionIsThrown()
		{
			var value = 0;
			var decimals = 2;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);
		}

		[TestMethod]
		public void TryParse_ZeroImpliedDecimals_CouldParse()
		{
			var value = "10050";
			var decimals = 0;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(10050m, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_OneImpliedDecimals_CouldParse()
		{
			var value = "10025";
			var decimals = 1;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(1002.5m, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NegativeImpliedDecimals_CouldNotParse()
		{
			var value = "10025";
			var decimals = -1;
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DecimalField));
			var bitAttribute = new ImplicitDecimalAttribute(decimals);
			var couldParse = bitAttribute.TryParse(property, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		private class MockRecord
		{
			public decimal DecimalField { get; set; }
			public double DoubleField { get; set; }
			public float FloatField { get; set; }
			public decimal? NullableDecimalField { get; set; }
		}
	}
}
