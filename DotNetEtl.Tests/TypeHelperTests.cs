using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class TypeHelperTests
	{
		[TestMethod]
		public void IsNumeric_Byte_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(byte));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_SByte_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(sbyte));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_Short_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(short));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_UShort_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(ushort));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_Int_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(int));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_UInt_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(uint));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_Long_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(long));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_ULong_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(ulong));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_Float_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(float));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_Double_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(double));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_Decimal_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(decimal));

			Assert.IsTrue(isNumeric);
		}
		
		[TestMethod]
		public void IsNumeric_NullableByte_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(byte?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableSByte_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(sbyte?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableShort_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(short?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableUShort_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(ushort?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableInt_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(int?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableUInt_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(uint?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableLong_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(long?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableULong_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(ulong?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableFloat_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(float?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableDouble_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(double?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_NullableDecimal_ReturnsTrue()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(decimal?));

			Assert.IsTrue(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_Object_ReturnsFalse()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(object));

			Assert.IsFalse(isNumeric);
		}

		[TestMethod]
		public void IsNumeric_String_ReturnsFalse()
		{
			var isNumeric = TypeHelper.IsNumeric(typeof(string));

			Assert.IsFalse(isNumeric);
		}
	}
}
