using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class DatabaseWriterCommandFactoryTests
	{
		[TestMethod]
		public void Constructor_Default_DefaultValuesAreSet()
		{
			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory();

			Assert.IsNull(databaseWriterCommandFactory.CommandText);
			Assert.AreEqual(CommandType.StoredProcedure, databaseWriterCommandFactory.CommandType);
			Assert.AreEqual(TimeSpan.FromSeconds(30), databaseWriterCommandFactory.CommandTimeout);
		}

		[TestMethod]
		public void Constructor_CommandTypeAndCommandText_CommandTypeAndCommandTextPropertiesAreSet()
		{
			var commandText = "exec dbo.InsertPerson";
			var commandType = CommandType.Text;

			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory(commandType, commandText);

			Assert.AreEqual(commandText, databaseWriterCommandFactory.CommandText);
			Assert.AreEqual(commandType, databaseWriterCommandFactory.CommandType);
			Assert.AreEqual(TimeSpan.FromSeconds(30), databaseWriterCommandFactory.CommandTimeout);
		}

		[TestMethod]
		public void Constructor_CommandParameterProvider_CommandParameterProviderIsUsed()
		{
			var record = Person.Faker.Generate(1).Single();
			var commandParameterProvider = MockRepository.GenerateMock<IDatabaseWriterCommandParameterProvider>();

			commandParameterProvider.Expect(x => x.GetParameters(Arg<object>.Is.Equal(record))).Return(new SqlParameter[0]).Repeat.Once();

			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory(commandParameterProvider);

			using (var command = databaseWriterCommandFactory.Create(record))
			{
				commandParameterProvider.VerifyAllExpectations();
			}
		}

		[TestMethod]
		public void Constructor_CommandParameterProviderAndCommandTypeAndCommandText_CommandParameterProviderIsUsedAndPropertiesAreSet()
		{
			var commandText = "exec dbo.InsertPerson";
			var commandType = CommandType.Text;
			var record = Person.Faker.Generate(1).Single();
			var commandParameterProvider = MockRepository.GenerateMock<IDatabaseWriterCommandParameterProvider>();

			commandParameterProvider.Expect(x => x.GetParameters(Arg<object>.Is.Equal(record))).Return(new SqlParameter[0]).Repeat.Once();

			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory(commandParameterProvider, commandType, commandText);

			using (var command = databaseWriterCommandFactory.Create(record))
			{
				commandParameterProvider.VerifyAllExpectations();
			}

			Assert.AreEqual(commandText, databaseWriterCommandFactory.CommandText);
			Assert.AreEqual(commandType, databaseWriterCommandFactory.CommandType);
			Assert.AreEqual(TimeSpan.FromSeconds(30), databaseWriterCommandFactory.CommandTimeout);
		}

		[TestMethod]
		public void Constructor_CommandParameterProviderAndCommandText_CommandParameterProviderIsUsedAndCommandTextPropertyIsSet()
		{
			var commandText = "dbo.InsertPerson";
			var record = Person.Faker.Generate(1).Single();
			var commandParameterProvider = MockRepository.GenerateMock<IDatabaseWriterCommandParameterProvider>();

			commandParameterProvider.Expect(x => x.GetParameters(Arg<object>.Is.Equal(record))).Return(new SqlParameter[0]).Repeat.Once();

			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory(commandParameterProvider, commandText);

			using (var command = databaseWriterCommandFactory.Create(record))
			{
				commandParameterProvider.VerifyAllExpectations();
			}

			Assert.AreEqual(commandText, databaseWriterCommandFactory.CommandText);
			Assert.AreEqual(CommandType.StoredProcedure, databaseWriterCommandFactory.CommandType);
			Assert.AreEqual(TimeSpan.FromSeconds(30), databaseWriterCommandFactory.CommandTimeout);
		}

		[TestMethod]
		public void Create_DefaultConstructorAndRecordWithSqlParameterAttributes_CommandIsCreatedAsExpected()
		{
			var record = Person.Faker.Generate(1).Single();
			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory()
			{
				CommandText = "dbo.InsertPerson",
				CommandType = CommandType.StoredProcedure,
				CommandTimeout = TimeSpan.FromMinutes(2)
			};

			using (var command = databaseWriterCommandFactory.Create(record))
			{
				Assert.AreEqual(databaseWriterCommandFactory.CommandText, command.CommandText);
				Assert.AreEqual(databaseWriterCommandFactory.CommandType, command.CommandType);
				Assert.AreEqual(databaseWriterCommandFactory.CommandTimeout, TimeSpan.FromMilliseconds(command.CommandTimeout));

				DatabaseWriterCommandFactoryTests.AssertCommandParametersAreCreatedAsExpected(command, record);
			}
		}

		[TestMethod]
		public void Create_StaticCommandParameters_CommandIsCreatedWithStaticParameters()
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
			var commandType = CommandType.StoredProcedure;
			var commandText = "dbo.InsertPerson";
			var staticCommandParameters = DatabaseWriterCommandFactoryTests.CreateStaticParameters(record);
			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory(staticCommandParameters, commandType, commandText);

			using (var command = databaseWriterCommandFactory.Create(record))
			{
				Assert.AreEqual(databaseWriterCommandFactory.CommandText, command.CommandText);
				Assert.AreEqual(databaseWriterCommandFactory.CommandType, command.CommandType);
				Assert.AreEqual(databaseWriterCommandFactory.CommandTimeout, TimeSpan.FromMilliseconds(command.CommandTimeout));

				DatabaseWriterCommandFactoryTests.AssertCommandParametersAreCreatedAsExpected(command, record);
			}
		}

		[TestMethod]
		public void Create_CustomCommandParameterProvider_CommandIsCreatedAsExpected()
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
			var staticCommandParameters = DatabaseWriterCommandFactoryTests.CreateStaticParameters(record);

			var commandParameterProvider = MockRepository.GenerateMock<IDatabaseWriterCommandParameterProvider>();
			
			commandParameterProvider.Expect(x => x.GetParameters(Arg<object>.Is.Equal(record))).Return(staticCommandParameters).Repeat.Once();

			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory(commandParameterProvider)
			{
				CommandText = "dbo.InsertPerson",
				CommandType = CommandType.StoredProcedure,
				CommandTimeout = TimeSpan.FromMinutes(2)
			};

			using (var command = databaseWriterCommandFactory.Create(record))
			{
				commandParameterProvider.VerifyAllExpectations();

				Assert.AreEqual(databaseWriterCommandFactory.CommandText, command.CommandText);
				Assert.AreEqual(databaseWriterCommandFactory.CommandType, command.CommandType);
				Assert.AreEqual(databaseWriterCommandFactory.CommandTimeout, TimeSpan.FromMilliseconds(command.CommandTimeout));

				DatabaseWriterCommandFactoryTests.AssertCommandParametersAreCreatedAsExpected(command, record);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_CommandParameterProviderThrowsException_ExceptionIsPropogated()
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
			var staticCommandParameters = DatabaseWriterCommandFactoryTests.CreateStaticParameters(record);

			var commandParameterProvider = MockRepository.GenerateMock<IDatabaseWriterCommandParameterProvider>();

			commandParameterProvider.Stub(x => x.GetParameters(Arg<object>.Is.Equal(record))).Throw(new InternalTestFailureException());

			var databaseWriterCommandFactory = new DatabaseWriterCommandFactory(commandParameterProvider)
			{
				CommandText = "dbo.InsertPerson",
				CommandType = CommandType.StoredProcedure,
				CommandTimeout = TimeSpan.FromMinutes(2)
			};

			databaseWriterCommandFactory.Create(record);
		}

		private static void AssertCommandParametersAreCreatedAsExpected(SqlCommand command, dynamic record)
		{
			var firstNameParameter = command.Parameters["FirstName"];
			var lastNameParameter = command.Parameters["LastName"];
			var middleInitialParameter = command.Parameters["MiddleInitial"];
			var ageParameter = command.Parameters["Age"];
			var dateOfBirthParameter = command.Parameters["DateOfBirth"];
			var genderParameter = command.Parameters["Gender"];

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
