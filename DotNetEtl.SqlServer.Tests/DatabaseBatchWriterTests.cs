using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DynamicDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class DatabaseBatchWriterTests
	{
		private static readonly string DbConnectionString = ConfigurationManager.ConnectionStrings["UnitTesting"].ConnectionString;

		[TestMethod]
		public void WriteRecord_OneRecord_RecordIsWritten()
		{
			var person = Person.Faker.Generate();

			var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString);
			var command = new SqlCommand("dbo.InsertPeople", connection);

			command.CommandType = CommandType.StoredProcedure;

			var tvpParameter = command.Parameters.Add("People", SqlDbType.Structured);
			tvpParameter.TypeName = "dbo.PersonType";

			using (var databaseBatchWriter = new DatabaseBatchWriter(command))
			{
				databaseBatchWriter.Open();
				databaseBatchWriter.WriteRecord(person);
				databaseBatchWriter.Commit();
			}

			using (var testDb = new TestDb(DatabaseBatchWriterTests.DbConnectionString))
			{
				var personRecords = testDb.Delete("dbo.Person", person);

				Assert.AreEqual(1, personRecords.Length);
			}
		}

		[TestMethod]
		public void WriteRecord_MultipleRecords_RecordsAreWritten()
		{
			var person1 = Person.Faker.Generate();
			var person2 = Person.Faker.Generate();
			var person3 = Person.Faker.Generate();

			var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString);
			var command = new SqlCommand("dbo.InsertPeople", connection);

			command.CommandType = CommandType.StoredProcedure;

			var tvpParameter = command.Parameters.Add("People", SqlDbType.Structured);
			tvpParameter.TypeName = "dbo.PersonType";

			using (var databaseBatchWriter = new DatabaseBatchWriter(command))
			{
				databaseBatchWriter.Open();
				databaseBatchWriter.WriteRecord(person1);
				databaseBatchWriter.WriteRecord(person2);
				databaseBatchWriter.WriteRecord(person3);
				databaseBatchWriter.Commit();
			}

			using (var testDb = new TestDb(DatabaseBatchWriterTests.DbConnectionString))
			{
				var personRecords = testDb.Delete("dbo.Person", person1, person2, person3);

				Assert.AreEqual(3, personRecords.Length);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(SqlException))]
		public void WriteRecord_BadRecord_ExceptionIsThrown()
		{
			var person = new Person();

			var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString);
			var command = new SqlCommand("dbo.InsertPeople", connection);

			command.CommandType = CommandType.StoredProcedure;

			var tvpParameter = command.Parameters.Add("People", SqlDbType.Structured);
			tvpParameter.TypeName = "dbo.PersonType";

			using (var databaseBatchWriter = new DatabaseBatchWriter(command))
			{
				databaseBatchWriter.Open();
				databaseBatchWriter.WriteRecord(person);

				try
				{
					databaseBatchWriter.Commit();
				}
				catch (AggregateException ae)
				{
					throw ae.InnerException;
				}

				Assert.Fail();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WriteRecord_MissingTvpParameter_ExceptionIsThrown()
		{
			var person = Person.Faker.Generate();

			var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString);
			var command = new SqlCommand("dbo.InsertPeople", connection);

			command.CommandType = CommandType.StoredProcedure;
				
			using (var databaseBatchWriter = new DatabaseBatchWriter(command))
			{
				databaseBatchWriter.Open();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WriteRecord_SqlDbTypeIsNotStructured_ExceptionIsThrown()
		{
			var person = Person.Faker.Generate();

			var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString);
			var command = new SqlCommand("dbo.InsertPeople", connection);

			command.CommandType = CommandType.StoredProcedure;

			var tvpParameter = command.Parameters.Add("People", SqlDbType.Int);
			tvpParameter.TypeName = "dbo.PersonType";

			using (var databaseBatchWriter = new DatabaseBatchWriter(command))
			{
				databaseBatchWriter.Open();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void WriteRecord_ConnectionNotProvidedOnCommand_ExceptionIsThrown()
		{
			var person = Person.Faker.Generate();

			var command = new SqlCommand("dbo.InsertPeople");

			command.CommandType = CommandType.StoredProcedure;

			var tvpParameter = command.Parameters.Add("People", SqlDbType.Int);
			tvpParameter.TypeName = "dbo.PersonType";

			using (var databaseBatchWriter = new DatabaseBatchWriter(command))
			{
				databaseBatchWriter.Open();
			}
		}

		[TestMethod]
		public void WriteRecord_TvpParameterNameIsProvided_RecordIsWritten()
		{
			var person = Person.Faker.Generate();

			var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString);
			var command = new SqlCommand("dbo.InsertPeople", connection);

			command.CommandType = CommandType.StoredProcedure;

			var tvpParameter = command.Parameters.Add("People", SqlDbType.Structured);
			tvpParameter.TypeName = "dbo.PersonType";

			using (var databaseBatchWriter = new DatabaseBatchWriter(command, "People"))
			{
				databaseBatchWriter.Open();
				databaseBatchWriter.WriteRecord(person);
				databaseBatchWriter.Commit();
			}

			using (var testDb = new TestDb(DatabaseBatchWriterTests.DbConnectionString))
			{
				var personRecords = testDb.Delete("dbo.Person", person);

				Assert.AreEqual(1, personRecords.Length);
			}
		}

		[TestMethod]
		public void WriteRecord_ShouldDisposeConnection_ConnectionIsDisposed()
		{
			var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString);
			var command = new SqlCommand("dbo.InsertPeople", connection);

			command.CommandType = CommandType.StoredProcedure;

			var tvpParameter = command.Parameters.Add("People", SqlDbType.Structured);
			tvpParameter.TypeName = "dbo.PersonType";

			using (var databaseBatchWriter = new DatabaseBatchWriter(command))
			{
				databaseBatchWriter.Open();
			}

			Assert.AreEqual(ConnectionState.Closed, connection.State);
		}

		[TestMethod]
		public void WriteRecord_ShouldNotDisposeConnection_ConnectionIsDisposed()
		{
			using (var connection = new SqlConnection(DatabaseBatchWriterTests.DbConnectionString))
			{
				var command = new SqlCommand("dbo.InsertPeople", connection);

				command.CommandType = CommandType.StoredProcedure;

				var tvpParameter = command.Parameters.Add("People", SqlDbType.Structured);
				tvpParameter.TypeName = "dbo.PersonType";

				using (var databaseBatchWriter = new DatabaseBatchWriter(command, disposeConnection: false))
				{
					databaseBatchWriter.Open();
				}

				Assert.AreEqual(ConnectionState.Open, connection.State);
			}
		}
	}
}
