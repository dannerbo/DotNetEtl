using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class TableValuedParameterFieldAttributeTests
	{
		[TestMethod]
		public void Constructor_OrdinalAndNameAndSqlDbTypeAreProvided_PropertiesAreSet()
		{
			var ordinal = 1;
			var name = "TestField";
			var sqlDbType = SqlDbType.Int;

			var tableValuedParameterFieldAttribute = new TableValuedParameterFieldAttribute(ordinal, name, sqlDbType);

			Assert.AreEqual(ordinal, tableValuedParameterFieldAttribute.Ordinal);
			Assert.AreEqual(name, tableValuedParameterFieldAttribute.Name);
			Assert.AreEqual(sqlDbType, tableValuedParameterFieldAttribute.SqlDbType);
			Assert.IsNull(tableValuedParameterFieldAttribute.Precision);
			Assert.IsNull(tableValuedParameterFieldAttribute.Scale);
			Assert.IsNull(tableValuedParameterFieldAttribute.MaxLength);
		}

		[TestMethod]
		public void Constructor_OrdinalAndNameAndSqlDbTypeAndPrecisionAndScaleAreProvided_PropertiesAreSet()
		{
			var ordinal = 1;
			var name = "TestField";
			var sqlDbType = SqlDbType.Decimal;
			var precision = (byte)8;
			var scale = (byte)2;

			var tableValuedParameterFieldAttribute = new TableValuedParameterFieldAttribute(ordinal, name, sqlDbType, precision, scale);

			Assert.AreEqual(ordinal, tableValuedParameterFieldAttribute.Ordinal);
			Assert.AreEqual(name, tableValuedParameterFieldAttribute.Name);
			Assert.AreEqual(sqlDbType, tableValuedParameterFieldAttribute.SqlDbType);
			Assert.AreEqual(precision, tableValuedParameterFieldAttribute.Precision);
			Assert.AreEqual(scale, tableValuedParameterFieldAttribute.Scale);
			Assert.IsNull(tableValuedParameterFieldAttribute.MaxLength);

		}

		[TestMethod]
		public void Constructor_OrdinalAndNameAndSqlDbTypeAndMaxLengthAreProvided_PropertiesAreSet()
		{
			var ordinal = 1;
			var name = "TestField";
			var sqlDbType = SqlDbType.VarChar;
			var maxLength = 50;

			var tableValuedParameterFieldAttribute = new TableValuedParameterFieldAttribute(ordinal, name, sqlDbType, maxLength);

			Assert.AreEqual(ordinal, tableValuedParameterFieldAttribute.Ordinal);
			Assert.AreEqual(name, tableValuedParameterFieldAttribute.Name);
			Assert.AreEqual(sqlDbType, tableValuedParameterFieldAttribute.SqlDbType);
			Assert.AreEqual(maxLength, tableValuedParameterFieldAttribute.MaxLength);
			Assert.IsNull(tableValuedParameterFieldAttribute.Precision);
			Assert.IsNull(tableValuedParameterFieldAttribute.Scale);
		}
	}
}
