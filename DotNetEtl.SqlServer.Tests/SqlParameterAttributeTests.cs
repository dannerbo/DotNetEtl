using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class SqlParameterAttributeTests
	{
		[TestMethod]
		public void Constructor_NameIsProvided_NamePropertyIsSet()
		{
			var name = "TestParameter";

			var sqlParameterAttribute = new SqlParameterAttribute(name);

			Assert.AreEqual(name, sqlParameterAttribute.Name);
			Assert.IsNull(sqlParameterAttribute.SqlDbType);
			Assert.IsNull(sqlParameterAttribute.Precision);
			Assert.IsNull(sqlParameterAttribute.Scale);
			Assert.IsNull(sqlParameterAttribute.Size);
		}

		[TestMethod]
		public void Constructor_NameAndSqlDbTypeAreProvided_NameAndSqlDbTypePropertiesAreSet()
		{
			var name = "TestParameter";
			var sqlDbType = SqlDbType.Int;

			var sqlParameterAttribute = new SqlParameterAttribute(name, sqlDbType);

			Assert.AreEqual(name, sqlParameterAttribute.Name);
			Assert.AreEqual(sqlDbType, sqlParameterAttribute.SqlDbType);
			Assert.IsNull(sqlParameterAttribute.Precision);
			Assert.IsNull(sqlParameterAttribute.Scale);
			Assert.IsNull(sqlParameterAttribute.Size);
		}

		[TestMethod]
		public void Constructor_NameAndSqlDbTypeAndSizeAreProvided_NameAndSqlDbTypeAndSizePropertiesAreSet()
		{
			var name = "TestParameter";
			var sqlDbType = SqlDbType.VarChar;
			var size = 50;

			var sqlParameterAttribute = new SqlParameterAttribute(name, sqlDbType, size);

			Assert.AreEqual(name, sqlParameterAttribute.Name);
			Assert.AreEqual(sqlDbType, sqlParameterAttribute.SqlDbType);
			Assert.AreEqual(size, sqlParameterAttribute.Size);
			Assert.IsNull(sqlParameterAttribute.Precision);
			Assert.IsNull(sqlParameterAttribute.Scale);
		}

		[TestMethod]
		public void Constructor_NameAndSqlDbTypeAndPrecisionAndScaleAreProvided_NameAndSqlDbTypeAndPrecisionAndScalePropertiesAreSet()
		{
			var name = "TestParameter";
			var sqlDbType = SqlDbType.Decimal;
			var precision = (byte)8;
			var scale = (byte)2;

			var sqlParameterAttribute = new SqlParameterAttribute(name, sqlDbType, precision, scale);

			Assert.AreEqual(name, sqlParameterAttribute.Name);
			Assert.AreEqual(sqlDbType, sqlParameterAttribute.SqlDbType);
			Assert.AreEqual(precision, sqlParameterAttribute.Precision);
			Assert.AreEqual(scale, sqlParameterAttribute.Scale);
			Assert.IsNull(sqlParameterAttribute.Size);
		}
	}
}
