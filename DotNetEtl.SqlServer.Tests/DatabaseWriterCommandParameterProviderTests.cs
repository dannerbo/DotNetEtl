using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class DatabaseWriterCommandParameterProviderTests
	{
		[TestMethod]
		public void GetParameters_RecordWithSqlParameterAttributes_ParametersAreCreatedAsExpected()
		{
			var record = Person.Faker.Generate(1).Single();

			var databaseWriterCommandParameterProvider = new DatabaseWriterCommandParameterProvider();

			var parameters = databaseWriterCommandParameterProvider.GetParameters(record);

			DatabaseWriterCommandParameterProviderTests.AssertCommandParametersAreCreatedAsExpected(parameters, record);
		}

		[TestMethod]
		public void GetParameters_StaticParameters_StaticParametersAreReturned()
		{
			var person = Person.Faker.Generate(1).Single();
			var record = new
			{
				person.FirstName,
				person.LastName,
				person.MiddleInitial,
				person.Age,
				person.DateOfBirth,
				person.Gender
			};
			var staticParameters = DatabaseWriterCommandParameterProviderTests.CreateStaticParameters(record);

			var databaseWriterCommandParameterProvider = new DatabaseWriterCommandParameterProvider(staticParameters);

			var parameters = databaseWriterCommandParameterProvider.GetParameters(record);

			DatabaseWriterCommandParameterProviderTests.AssertCommandParametersAreCreatedAsExpected(parameters, record);
		}

		private static void AssertCommandParametersAreCreatedAsExpected(IEnumerable<SqlParameter> parameters, dynamic record)
		{
			var firstNameParameter = parameters.Single(x => x.ParameterName.Equals("FirstName"));
			var lastNameParameter = parameters.Single(x => x.ParameterName.Equals("LastName"));
			var middleInitialParameter = parameters.Single(x => x.ParameterName.Equals("MiddleInitial"));
			var ageParameter = parameters.Single(x => x.ParameterName.Equals("Age"));
			var dateOfBirthParameter = parameters.Single(x => x.ParameterName.Equals("DateOfBirth"));
			var genderParameter = parameters.Single(x => x.ParameterName.Equals("Gender"));

			Assert.AreEqual(record.FirstName, firstNameParameter.Value);
			Assert.AreEqual(record.LastName, lastNameParameter.Value);
			Assert.AreEqual(record.MiddleInitial, middleInitialParameter.Value);
			Assert.AreEqual(record.Age, ageParameter.Value);
			Assert.AreEqual(record.DateOfBirth, dateOfBirthParameter.Value);
			Assert.AreEqual(record.Gender, genderParameter.Value);

			Assert.AreEqual(SqlDbType.VarChar, firstNameParameter.SqlDbType);
			Assert.AreEqual(SqlDbType.VarChar, lastNameParameter.SqlDbType);
			Assert.AreEqual(SqlDbType.Char, middleInitialParameter.SqlDbType);
			Assert.AreEqual(SqlDbType.TinyInt, ageParameter.SqlDbType);
			Assert.AreEqual(SqlDbType.Date, dateOfBirthParameter.SqlDbType);
			Assert.AreEqual(SqlDbType.TinyInt, genderParameter.SqlDbType);

			Assert.AreEqual(50, firstNameParameter.Size);
			Assert.AreEqual(50, lastNameParameter.Size);
			Assert.AreEqual(1, middleInitialParameter.Size);
		}

		private static IEnumerable<SqlParameter> CreateStaticParameters(dynamic record)
		{
			var parameters = new List<SqlParameter>()
			{
				new SqlParameter("FirstName", SqlDbType.VarChar, 50),
				new SqlParameter("LastName", SqlDbType.VarChar, 50),
				new SqlParameter("MiddleInitial", SqlDbType.Char, 1),
				new SqlParameter("Age", SqlDbType.TinyInt),
				new SqlParameter("DateOfBirth", SqlDbType.Date),
				new SqlParameter("Gender", SqlDbType.TinyInt)
			};

			parameters[0].Value = record.FirstName;
			parameters[1].Value = record.LastName;
			parameters[2].Value = record.MiddleInitial;
			parameters[3].Value = record.Age;
			parameters[4].Value = record.DateOfBirth;
			parameters[5].Value = record.Gender;

			return parameters;
		}
	}
}
