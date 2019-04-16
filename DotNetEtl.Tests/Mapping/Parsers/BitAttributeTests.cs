using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Parsers.Tests
{
	[TestClass]
	public class BitAttributeTests
	{
		[TestMethod]
		public void TryParse_StringValue_CouldNotParse()
		{
			var value = "test";
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_NullStringValue_CouldNotParse()
		{
			var value = (string)null;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_TrueBooleanValue_CouldParseAndTrueReturned()
		{
			var value = true;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(true, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_FalseBooleanValue_CouldParseAndFalseReturned()
		{
			var value = false;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(false, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NullBooleanValue_CouldNotParse()
		{
			var value = (bool?)null;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_ZeroIntValue_CouldParseAndFalseReturned()
		{
			var value = 0;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(false, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_OneIntValue_CouldParseAndTrueReturned()
		{
			var value = 1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(true, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NegativeIntValue_CouldNotParse()
		{
			var value = -1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_GreaterThanOneIntValue_CouldNotParse()
		{
			var value = 2;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}
		
		[TestMethod]
		public void TryParse_ZeroNullableIntValue_CouldParseAndFalseReturned()
		{
			var value = (int?)0;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(false, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_OneNullableIntValue_CouldParseAndTrueReturned()
		{
			var value = (int?)1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(true, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NegativeNullableIntValue_CouldNotParse()
		{
			var value = (int?)-1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_GreaterThanOneNullableIntValue_CouldNotParse()
		{
			var value = (int?)2;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_NullNullableIntValue_CouldNotParse()
		{
			var value = (int?)null;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}
		
		[TestMethod]
		public void TryParse_ZeroShortValue_CouldParseAndFalseReturned()
		{
			var value = (short)0;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(false, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_OneShortValue_CouldParseAndTrueReturned()
		{
			var value = (short)1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(true, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NegativeShortValue_CouldNotParse()
		{
			var value = (short)-1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_GreaterThanOneShortValue_CouldNotParse()
		{
			var value = (short)2;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}
		
		[TestMethod]
		public void TryParse_ZeroDecimalValue_CouldParseAndFalseReturned()
		{
			var value = (decimal)0;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(false, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_OneDecimalValue_CouldParseAndTrueReturned()
		{
			var value = (decimal)1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(true, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NegativeDecimalValue_CouldNotParse()
		{
			var value = (decimal)-1;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		public void TryParse_GreaterThanOneDecimalValue_CouldNotParse()
		{
			var value = (decimal)2;
			var bitAttribute = new BitAttribute();
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}
		
		[TestMethod]
		public void TryParse_TrueBooleanValueWithCustomTrueValue_CouldParseAndCustomTrueValueReturned()
		{
			var value = true;
			var bitAttribute = new BitAttribute() { TrueValue = "YES" };
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual("YES", parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_TrueBooleanValueWithCustomFalseValue_CouldParseAndCustomFalseValueReturned()
		{
			var value = false;
			var bitAttribute = new BitAttribute() { FalseValue = "NO" };
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual("NO", parsedFieldValue);
			Assert.IsNull(failureMessage);
		}
	}
}
