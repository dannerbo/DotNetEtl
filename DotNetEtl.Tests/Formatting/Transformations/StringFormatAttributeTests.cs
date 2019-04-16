using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class StringFormatAttributeTests
	{
		[TestMethod]
		public void ToString_Date_FormattedDateReturned()
		{
			var value = DateTime.Parse("2000-12-01");
			var format = "yyyyMMdd";
			var stringFormatAttribute = new StringFormatAttribute(format);
			var transformedValue = stringFormatAttribute.ToString(value);

			Assert.AreEqual("20001201", transformedValue);
		}

		[TestMethod]
		public void ToString_DateWithCurrencyFormat_CurrencyFormatStringReturned()
		{
			var value = DateTime.Parse("2000-12-01");
			var format = "C2";
			var stringFormatAttribute = new StringFormatAttribute(format);
			var transformedValue = stringFormatAttribute.ToString(value);

			Assert.AreEqual(format, transformedValue);
		}
		
		[TestMethod]
		public void ToString_NullValue_EmptyStringReturned()
		{
			var format = "yyyyMMdd";
			var stringFormatAttribute = new StringFormatAttribute(format);
			var transformedValue = stringFormatAttribute.ToString(null);

			Assert.AreEqual(String.Empty, transformedValue);
		}
	}
}
