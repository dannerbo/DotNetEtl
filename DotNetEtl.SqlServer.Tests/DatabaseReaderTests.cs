using System.Collections.Generic;
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
	public class DatabaseReaderTests
	{
		private static readonly string DbConnectionString = ConfigurationManager.ConnectionStrings["UnitTesting"].ConnectionString;

		[TestMethod]
		public void TryReadRecord_ZeroRecordsAvailable_ZeroRecordsAreRead()
		{
			var connection = new SqlConnection(DatabaseReaderTests.DbConnectionString);
			var command = new SqlCommand("SELECT * FROM dbo.Person", connection);

			using (var databaseReader = new DatabaseReader<Person>(command))
			{
				databaseReader.Open();

				var tryReadRecordResult1 = databaseReader.TryReadRecord(out var recordRead1, out var failures1);

				Assert.IsFalse(tryReadRecordResult1);
			}
		}

		[TestMethod]
		public void TryReadRecord_OneRecordIsAvailable_OneRecordIsRead()
		{
			var person1 = Person.Faker.Generate();

			using (var testDb = new TestDb(DatabaseReaderTests.DbConnectionString))
			{
				var personRecord1 = testDb.Insert("dbo.Person", true, person1);

				var connection = new SqlConnection(DatabaseReaderTests.DbConnectionString);
				var command = new SqlCommand("SELECT * FROM dbo.Person", connection);

				using (var databaseReader = new DatabaseReader<Person>(command))
				{
					databaseReader.Open();

					var tryReadRecordResult1 = databaseReader.TryReadRecord(out var recordRead1, out var failures1);
					var tryReadRecordResult2 = databaseReader.TryReadRecord(out var recordRead2, out var failures2);

					Assert.IsTrue(tryReadRecordResult1);
					Assert.IsTrue(person1.Equals(recordRead1));
					Assert.IsFalse(tryReadRecordResult2);
				}
			}
		}

		[TestMethod]
		public void TryReadRecord_MultipleRecordsAreAvailable_MultipleRecordsAreRead()
		{
			var person1 = Person.Faker.Generate();
			var person2 = Person.Faker.Generate();
			var person3 = Person.Faker.Generate();

			using (var testDb = new TestDb(DatabaseReaderTests.DbConnectionString))
			{
				var personRecord1 = testDb.Insert("dbo.Person", true, person1, person2, person3);

				var connection = new SqlConnection(DatabaseReaderTests.DbConnectionString);
				var command = new SqlCommand("SELECT * FROM dbo.Person", connection);

				using (var databaseReader = new DatabaseReader<Person>(command))
				{
					databaseReader.Open();

					var tryReadRecordResult1 = databaseReader.TryReadRecord(out var recordRead1, out var failures1);
					var tryReadRecordResult2 = databaseReader.TryReadRecord(out var recordRead2, out var failures2);
					var tryReadRecordResult3 = databaseReader.TryReadRecord(out var recordRead3, out var failures3);
					var tryReadRecordResult4 = databaseReader.TryReadRecord(out var recordRead4, out var failures4);

					Assert.IsTrue(tryReadRecordResult1);
					Assert.IsTrue(person1.Equals(recordRead1));
					Assert.IsTrue(tryReadRecordResult2);
					Assert.IsTrue(person2.Equals(recordRead2));
					Assert.IsTrue(tryReadRecordResult3);
					Assert.IsTrue(person3.Equals(recordRead3));
					Assert.IsFalse(tryReadRecordResult4);
				}
			}
		}

		[TestMethod]
		public void TryReadRecord_ShouldDisposeConnection_ConnectionIsDisposed()
		{
			var connection = new SqlConnection(DatabaseReaderTests.DbConnectionString);
			var command = new SqlCommand("SELECT * FROM dbo.Person", connection);

			using (var databaseReader = new DatabaseReader<Person>(command))
			{
				databaseReader.Open();
			}

			Assert.AreEqual(ConnectionState.Closed, connection.State);
		}

		[TestMethod]
		public void TryReadRecord_ShouldNotDisposeConnection_ConnectionIsNotDisposed()
		{
			using (var connection = new SqlConnection(DatabaseReaderTests.DbConnectionString))
			{
				var command = new SqlCommand("SELECT * FROM dbo.Person", connection);

				using (var databaseReader = new DatabaseReader<Person>(command, disposeConnection: false))
				{
					databaseReader.Open();
				}

				Assert.AreEqual(ConnectionState.Open, connection.State);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryReadRecord_RecordMapperThrowsException_ExceptionIsPropogated()
		{
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var person = Person.Faker.Generate();

			using (var testDb = new TestDb(DatabaseReaderTests.DbConnectionString))
			{
				testDb.Insert("dbo.Person", true, person);

				var connection = new SqlConnection(DatabaseReaderTests.DbConnectionString);
				var command = new SqlCommand("SELECT * FROM dbo.Person", connection);

				using (var databaseReader = new DatabaseReader(recordMapper, command))
				{
					databaseReader.Open();
					databaseReader.TryReadRecord(out var record, out var failures);
				}
			}
		}

		[TestMethod]
		public void TryReadRecord_RecordMapperFailure_RecordIsNotReadAndFailureIsReturned()
		{
			var failure = new FieldFailure()
			{
				FieldName = nameof(Person.DateOfBirth),
				Message = "Field is invalid."
			};

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Anything,
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(new FieldFailure[] { failure }).Dummy))
				.Return(false);

			var person = Person.Faker.Generate();

			using (var testDb = new TestDb(DatabaseReaderTests.DbConnectionString))
			{
				testDb.Insert("dbo.Person", true, person);

				var connection = new SqlConnection(DatabaseReaderTests.DbConnectionString);
				var command = new SqlCommand("SELECT * FROM dbo.Person", connection);

				using (var databaseReader = new DatabaseReader(recordMapper, command))
				{
					databaseReader.Open();

					var couldReadRecord = databaseReader.TryReadRecord(out var record, out var failures);

					Assert.IsFalse(couldReadRecord);
					Assert.AreEqual(failure, failures.Single());
				}
			}
		}
	}
}
