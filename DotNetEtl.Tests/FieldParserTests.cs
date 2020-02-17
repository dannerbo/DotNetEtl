using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class FieldParserTests
	{
		[TestMethod]
		public void TryParse_ParsableStringFieldWithParseAttribute_ParsedFieldIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.ParsableStringFieldWithParseAttribute));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "1", out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual("A", parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_UnparsableStringFieldWithParseAttribute_ParseFails()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.UnparsableStringFieldWithParseAttribute));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "1", out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.AreEqual("Could not parse.", failureMessage);
		}

		[TestMethod]
		public void TryParse_IntFieldWithoutParseAttribute_ParsedFieldIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.IntFieldWithoutParseAttribute));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "1", out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(1, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}
		
		[TestMethod]
		public void TryParse_NullableIntFieldWithoutParseAttribute_ParsedFieldIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableIntFieldWithoutParseAttribute));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "1", out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(1, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryParse_ParseAttributeTryParseThrowsException_ExceptionIsPropogated()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.ExceptionThrowingField));
			var fieldParser = new FieldParser();

			fieldParser.TryParse(property, "1", out var parsedFieldValue, out var failureMessage);
		}

		[TestMethod]
		public void TryParse_ParsableEnumFieldWithIntValue_ParsedFieldIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.EnumField));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "1", out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(MockEnum.One, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_ParsableEnumFieldWithStringValue_ParsedFieldIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.EnumField));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "One", out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(MockEnum.One, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_EnumFieldWithInvalidIntValue_ParsedFieldIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.EnumField));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "4", out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual((MockEnum)4, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}
		
		[TestMethod]
		public void TryParse_EnumFieldWithInvalidStringValue_ParseFails()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.EnumField));
			var fieldParser = new FieldParser();

			var couldParse = fieldParser.TryParse(property, "Four", out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.AreEqual("Field is invalid.", failureMessage);
		}

		private class MockRecord
		{
			[MockParseField(parsedFieldValue: "A")]
			public string ParsableStringFieldWithParseAttribute { get; set; }
			[MockParseField(failureMessage: "Could not parse.")]
			public string UnparsableStringFieldWithParseAttribute { get; set; }
			public int IntFieldWithoutParseAttribute { get; set; }
			public int? NullableIntFieldWithoutParseAttribute { get; set; }
			[ExceptionThrowingParseField]
			public string ExceptionThrowingField { get; set; }
			public MockEnum EnumField { get; set; }
		}

		private enum MockEnum
		{
			Undefined,
			One,
			Two,
			Three
		}

		private class MockParseFieldAttribute : ParseFieldAttribute
		{
			private object parsedFieldValue;
			private string failureMessage;

			public MockParseFieldAttribute(object parsedFieldValue)
			{
				this.parsedFieldValue = parsedFieldValue;
			}

			public MockParseFieldAttribute(string failureMessage)
			{
				this.failureMessage = failureMessage;
			}

			public override bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
			{
				if (this.failureMessage != null)
				{
					parsedFieldValue = null;
					failureMessage = this.failureMessage;

					return false;
				}

				parsedFieldValue = this.parsedFieldValue;
				failureMessage = null;

				return true;
			}
		}

		private class ExceptionThrowingParseFieldAttribute : ParseFieldAttribute
		{
			public override bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
			{
				throw new InternalTestFailureException();
			}
		}
	}
}
