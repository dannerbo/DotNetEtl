using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class BinaryFieldParserTests
	{
		[TestMethod]
		public void TryParse_BoolWithFalseValue_BoolIsReturned()
		{
			var bytes = new byte[] { 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Bool));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(false, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_BoolWithTrueValue_BoolIsReturned()
		{
			var bytes = new byte[] { 1 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Bool));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(true, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidBool_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Bool));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Byte_ByteIsReturned()
		{
			var bytes = new byte[] { 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Byte));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(bytes[0], parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidByte_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Byte));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Bytes_BytesAreReturned()
		{
			var bytes = new byte[2] { 100, 200 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Bytes));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(bytes[0], ((byte[])parsedFieldValue)[0]);
			Assert.AreEqual(bytes[1], ((byte[])parsedFieldValue)[1]);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Char_CharIsReturned()
		{
			var bytes = new byte[] { 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Char));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual((char)100, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidChar_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Char));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Chars_CharsAreReturned()
		{
			var bytes = new byte[2] { 100, 101 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Chars));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual((char)bytes[0], ((char[])parsedFieldValue)[0]);
			Assert.AreEqual((char)bytes[1], ((char[])parsedFieldValue)[1]);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Decimal_DecimalIsReturned()
		{
			var bytes = new byte[] { 200, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Decimal));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(2.00m, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidDecimal_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Decimal));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Double_DoubleIsReturned()
		{
			var bytes = new byte[] { 0, 0, 0, 0, 0, 0, 37, 64 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Double));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(10.5d, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidDouble_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Double));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Short_ShortIsReturned()
		{
			var bytes = new byte[] { 100, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Short));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual((short)100, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidShort_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Short));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}
		
		[TestMethod]
		public void TryParse_Int_IntIsReturned()
		{
			var bytes = new byte[] { 100, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Int));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidInt_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Int));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Long_LongIsReturned()
		{
			var bytes = new byte[] { 100, 0, 0, 0, 0, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Long));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100L, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidLong_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Long));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_SByte_SByteIsReturned()
		{
			var bytes = new byte[] { 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.SByte));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual((sbyte)100, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidSByte_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.SByte));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_Float_FloatIsReturned()
		{
			var bytes = new byte[] { 0, 0, 40, 65 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Float));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(10.5f, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidFloat_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Float));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_String_StringIsReturned()
		{
			var bytes = new byte[] { 116, 101, 120, 116 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.String));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual("text", parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NullString_NullIsReturned()
		{
			var bytes = new byte[] { 0, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.String));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_UShort_UShortIsReturned()
		{
			var bytes = new byte[] { 100, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.UShort));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual((ushort)100, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidUShort_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.UShort));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_UInt_UIntIsReturned()
		{
			var bytes = new byte[] { 100, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.UInt));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual((uint)100, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidUInt_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.UInt));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_ULong_ULongIsReturned()
		{
			var bytes = new byte[] { 100, 0, 0, 0, 0, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.ULong));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100UL, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidULong_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.ULong));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_DateTime_DateTimeIsReturned()
		{
			var bytes = new byte[] { 0, 64, 228, 71, 2, 34, 193, 8 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DateTime));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(DateTime.Parse("2000-01-01"), parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_InvalidDateTime_FailureIsReturned()
		{
			var bytes = new byte[] { };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.DateTime));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNotNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NullableIntWithNonNullValue_NullableIntIsReturned()
		{
			var bytes = new byte[] { 100, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableInt));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual(100, parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		public void TryParse_NullableIntWithNullValue_NullIsReturned()
		{
			var bytes = new byte[] { 0, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.NullableInt));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsFalse(couldParse);
			Assert.IsNull(parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TryParse_NonSupportedType_ExceptionIsThrown()
		{
			var bytes = new byte[] { 100, 0, 0, 0 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.Object));

			var binaryFieldParser = new BinaryFieldParser();

			binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);
		}

		[TestMethod]
		public void TryParse_FieldWithParseFieldAttribute_ParseFieldAttributeResultIsReturned()
		{
			var bytes = new byte[] { 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithParseFieldAttribute));

			var binaryFieldParser = new BinaryFieldParser();

			var couldParse = binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);

			Assert.IsTrue(couldParse);
			Assert.AreEqual("parsed", parsedFieldValue);
			Assert.IsNull(failureMessage);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryParse_ParseFieldAttributeThrowsException_ExceptionIsPropogated()
		{
			var bytes = new byte[] { 100 };
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithFailingParseFieldAttribute));

			var binaryFieldParser = new BinaryFieldParser();

			binaryFieldParser.TryParse(property, bytes, out var parsedFieldValue, out var failureMessage);
		}

		private class MockRecord
		{
			public bool Bool { get; set; }
			public byte Byte { get; set; }
			public byte[] Bytes { get; set; }
			public char Char { get; set; }
			public char[] Chars { get; set; }
			public decimal Decimal { get; set; }
			public double Double { get; set; }
			public short Short { get; set; }
			public int Int { get; set; }
			public long Long { get; set; }
			public sbyte SByte { get; set; }
			public float Float { get; set; }
			public string String { get; set; }
			public ushort UShort { get; set; }
			public uint UInt { get; set; }
			public ulong ULong { get; set; }
			public DateTime DateTime { get; set; }
			public object Object { get; set; }
			public int? NullableInt { get; set; }
			[MockParseField("parsed")]
			public byte FieldWithParseFieldAttribute { get; set; }
			[MockFailingParseField]
			public byte FieldWithFailingParseFieldAttribute { get; set; }
		}

		private class MockParseFieldAttribute : ParseFieldAttribute
		{
			private object valueToReturn;

			public MockParseFieldAttribute(object valueToReturn)
			{
				this.valueToReturn = valueToReturn;
			}

			public override bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
			{
				parsedFieldValue = this.valueToReturn;
				failureMessage = null;

				return true;
			}
		}

		private class MockFailingParseFieldAttribute : ParseFieldAttribute
		{
			public override bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
			{
				throw new InternalTestFailureException();
			}
		}
	}
}
