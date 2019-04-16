using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class SqlDataRecordMapperTests
	{
		[TestMethod]
		public void Map_RecordWithTableValuedParameterFieldAttributes_SqlDataRecordIsReturned()
		{
			var record = Person.Faker.Generate(1).Single();

			var sqlDataRecordMapper = new SqlDataRecordMapper();

			var sqlDataRecord = sqlDataRecordMapper.Map(record);

			Assert.AreEqual(6, sqlDataRecord.FieldCount);
			Assert.AreEqual(record.FirstName, sqlDataRecord.GetValue(0));
			Assert.AreEqual(record.LastName, sqlDataRecord.GetValue(1));
			Assert.AreEqual(record.MiddleInitial, sqlDataRecord.GetValue(2));
			Assert.AreEqual((byte)record.Age, sqlDataRecord.GetValue(3));
			Assert.AreEqual(record.DateOfBirth, sqlDataRecord.GetValue(4));
			Assert.AreEqual((byte)record.Gender, sqlDataRecord.GetValue(5));

			var firstNameMetaData = sqlDataRecord.GetSqlMetaData(0);
			var lastNameMetaData = sqlDataRecord.GetSqlMetaData(1);
			var middleInitialMetaData = sqlDataRecord.GetSqlMetaData(2);
			var ageMetaData = sqlDataRecord.GetSqlMetaData(3);
			var dateOfBirthMetaData = sqlDataRecord.GetSqlMetaData(4);
			var genderMetaData = sqlDataRecord.GetSqlMetaData(5);

			Assert.AreEqual("FirstName", firstNameMetaData.Name);
			Assert.AreEqual(SqlDbType.VarChar, firstNameMetaData.SqlDbType);
			Assert.AreEqual(50, firstNameMetaData.MaxLength);

			Assert.AreEqual("LastName", lastNameMetaData.Name);
			Assert.AreEqual(SqlDbType.VarChar, lastNameMetaData.SqlDbType);
			Assert.AreEqual(50, lastNameMetaData.MaxLength);

			Assert.AreEqual("MiddleInitial", middleInitialMetaData.Name);
			Assert.AreEqual(SqlDbType.Char, middleInitialMetaData.SqlDbType);
			Assert.AreEqual(1, middleInitialMetaData.MaxLength);

			Assert.AreEqual("Age", ageMetaData.Name);
			Assert.AreEqual(SqlDbType.TinyInt, ageMetaData.SqlDbType);

			Assert.AreEqual("DateOfBirth", dateOfBirthMetaData.Name);
			Assert.AreEqual(SqlDbType.Date, dateOfBirthMetaData.SqlDbType);

			Assert.AreEqual("Gender", genderMetaData.Name);
			Assert.AreEqual(SqlDbType.TinyInt, genderMetaData.SqlDbType);
		}
	}
}
