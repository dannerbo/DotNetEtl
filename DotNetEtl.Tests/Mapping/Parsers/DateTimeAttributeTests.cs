using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Mapping.Parsers.Tests
{
	[TestClass]
	public class DateTimeAttributeTests
	{
		[TestMethod]
		public void TryParse_ValidDate_CouldParse()
		{
			var value = "20000101";
			var format = "yyyyMMdd";
			var bitAttribute = new DateTimeAttribute(format);
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(DateTime.Parse("2000-01-01"), (DateTime)parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidDate_CouldNotParse()
		{
			var value = "invalid";
			var format = "yyyyMMdd";
			var bitAttribute = new DateTimeAttribute(format);
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsFalse(String.IsNullOrEmpty(failureMessage));
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidCastException))]
		public void TryParse_NonStringValue_ExceptionIsThrown()
		{
			var value = 20000101;
			var format = "yyyyMMdd";
			var bitAttribute = new DateTimeAttribute(format);
			var couldParse = bitAttribute.TryParse(null, value, out object parsedFieldValue, out string failureMessage);
		}
	}
}
