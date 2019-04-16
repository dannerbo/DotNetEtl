using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DynamicDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class DatabaseWriterTests
	{
		private static readonly string DbConnectionString = ConfigurationManager.ConnectionStrings["UnitTesting"].ConnectionString;

		[TestMethod]
		public void WriteRecord_MultipleRecords_CommandIsExecutedForEachRecord()
		{
			var people = Person.Faker.Generate(2);

			var commands = new SqlCommand[]
			{
				DatabaseWriterTests.GenerateInsertPersonCommand(people[0]),
				DatabaseWriterTests.GenerateInsertPersonCommand(people[1])
			};

			var commandFactory = MockRepository.GenerateMock<IDatabaseWriterCommandFactory>();

			commandFactory.Stub(x => x.Create(Arg<object>.Is.Equal(people[0]))).Return(commands[0]);
			commandFactory.Stub(x => x.Create(Arg<object>.Is.Equal(people[1]))).Return(commands[1]);

			using (var databaseWriter = new DatabaseWriter(commandFactory, DatabaseWriterTests.DbConnectionString))
			{
				databaseWriter.Open();

				databaseWriter.WriteRecord(people[0]);
				databaseWriter.WriteRecord(people[1]);

				databaseWriter.Commit();
			}

			using (var testDb = new TestDb(DatabaseWriterTests.DbConnectionString))
			{
				var records = testDb.Delete("dbo.Person", people.ToArray());

				Assert.AreEqual(2, records.Length);
			}
		}

		[TestMethod]
		public void WriteRecord_MultipleRecordsWithTransactionAndNoCommit_TransactionIsRolledBack()
		{
			var people = Person.Faker.Generate(2);

			var commands = new SqlCommand[]
			{
				DatabaseWriterTests.GenerateInsertPersonCommand(people[0]),
				DatabaseWriterTests.GenerateInsertPersonCommand(people[1])
			};

			var commandFactory = MockRepository.GenerateMock<IDatabaseWriterCommandFactory>();

			commandFactory.Stub(x => x.Create(Arg<object>.Is.Equal(people[0]))).Return(commands[0]);
			commandFactory.Stub(x => x.Create(Arg<object>.Is.Equal(people[1]))).Return(commands[1]);

			using (var databaseWriter = new DatabaseWriter(commandFactory, DatabaseWriterTests.DbConnectionString))
			{
				databaseWriter.Open();

				databaseWriter.WriteRecord(people[0]);
				databaseWriter.WriteRecord(people[1]);
			}

			using (var testDb = new TestDb(DatabaseWriterTests.DbConnectionString))
			{
				var records = testDb.Delete("dbo.Person", people.ToArray());

				Assert.AreEqual(0, records.Length);
			}
		}

		[TestMethod]
		public void WriteRecord_MultipleRecordsWithoutTransactionAndNoCommit_ChangesAreCommitted()
		{
			var people = Person.Faker.Generate(2);

			var commands = new SqlCommand[]
			{
				DatabaseWriterTests.GenerateInsertPersonCommand(people[0]),
				DatabaseWriterTests.GenerateInsertPersonCommand(people[1])
			};

			var commandFactory = MockRepository.GenerateMock<IDatabaseWriterCommandFactory>();

			commandFactory.Stub(x => x.Create(Arg<object>.Is.Equal(people[0]))).Return(commands[0]);
			commandFactory.Stub(x => x.Create(Arg<object>.Is.Equal(people[1]))).Return(commands[1]);

			using (var databaseWriter = new DatabaseWriter(commandFactory, DatabaseWriterTests.DbConnectionString, false))
			{
				databaseWriter.Open();

				databaseWriter.WriteRecord(people[0]);
				databaseWriter.WriteRecord(people[1]);
			}

			using (var testDb = new TestDb(DatabaseWriterTests.DbConnectionString))
			{
				var records = testDb.Delete("dbo.Person", people.ToArray());

				Assert.AreEqual(2, records.Length);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(SqlException))]
		public void WriteRecord_CommandRaisesError_ExceptionIsPropogated()
		{
			var person = Person.Faker.Generate(1).Single();

			var command = new SqlCommand("dbo.RaiseError")
			{
				CommandType = CommandType.StoredProcedure
			};

			var commandFactory = MockRepository.GenerateMock<IDatabaseWriterCommandFactory>();

			commandFactory.Stub(x => x.Create(Arg<object>.Is.Equal(person))).Return(command);

			using (var databaseWriter = new DatabaseWriter(commandFactory, DatabaseWriterTests.DbConnectionString))
			{
				databaseWriter.Open();

				databaseWriter.WriteRecord(person);
			}
		}

		private static SqlCommand GenerateInsertPersonCommand(Person person)
		{
			var command = new SqlCommand("dbo.InsertPerson");

			command.CommandType = CommandType.StoredProcedure;

			command.Parameters.AddWithValue("FirstName", person.FirstName);
			command.Parameters.AddWithValue("LastName", person.LastName);
			command.Parameters.AddWithValue("MiddleInitial", person.MiddleInitial ?? (object)DBNull.Value);
			command.Parameters.AddWithValue("Age", person.Age);
			command.Parameters.AddWithValue("DateOfBirth", person.DateOfBirth);
			command.Parameters.AddWithValue("Gender", (int)person.Gender);

			return command;
		}
	}
}
