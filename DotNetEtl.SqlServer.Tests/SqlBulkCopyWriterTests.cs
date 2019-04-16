using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using DynamicDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class SqlBulkCopyWriterTests
	{
		private static readonly string DbConnectionString = ConfigurationManager.ConnectionStrings["UnitTesting"].ConnectionString;
		private static readonly IEnumerable<SqlBulkCopyColumnMapping> ColumnMappings = new List<SqlBulkCopyColumnMapping>()
		{
			new SqlBulkCopyColumnMapping(nameof(Person.FirstName), "FirstName"),
			new SqlBulkCopyColumnMapping(nameof(Person.LastName), "LastName"),
			new SqlBulkCopyColumnMapping(nameof(Person.MiddleInitial), "MiddleInitial"),
			new SqlBulkCopyColumnMapping(nameof(Person.Age), "Age"),
			new SqlBulkCopyColumnMapping(nameof(Person.DateOfBirth), "DateOfBirth"),
			new SqlBulkCopyColumnMapping(nameof(Person.Gender), "Gender")
		};

		[TestMethod]
		public void SqlBulkCopyWriter_WriteMultipleRecordsWithCommit_RecordsAreWrittenToDatabase()
		{
			var records = Person.Faker.Generate(10);
			var columnMappingProvider = MockRepository.GenerateMock<ISqlBulkCopyColumnMappingProvider>();

			columnMappingProvider.Stub(x => x.GetColumnMappings()).Return(SqlBulkCopyWriterTests.ColumnMappings);

			using (var sqlBulkCopyWriter = new SqlBulkCopyWriter(columnMappingProvider, SqlBulkCopyWriterTests.DbConnectionString, "dbo.Person"))
			{
				sqlBulkCopyWriter.Open();

				foreach (var record in records)
				{
					sqlBulkCopyWriter.WriteRecord(record);
				}

				sqlBulkCopyWriter.Commit();
			}

			using (var testDb = new TestDb(SqlBulkCopyWriterTests.DbConnectionString))
			{
				var recordsInDb = testDb.Delete("dbo.Person", records.ToArray());

				Assert.AreEqual(records.Count, recordsInDb.Length);
			}
		}

		[TestMethod]
		public void SqlBulkCopyWriter_WriteMultipleRecordsWithoutCommit_RecordsAreNotWrittenToDatabase()
		{
			var records = Person.Faker.Generate(10);
			var columnMappingProvider = MockRepository.GenerateMock<ISqlBulkCopyColumnMappingProvider>();

			columnMappingProvider.Stub(x => x.GetColumnMappings()).Return(SqlBulkCopyWriterTests.ColumnMappings);

			using (var sqlBulkCopyWriter = new SqlBulkCopyWriter(columnMappingProvider, SqlBulkCopyWriterTests.DbConnectionString, "dbo.Person"))
			{
				sqlBulkCopyWriter.Open();

				foreach (var record in records)
				{
					sqlBulkCopyWriter.WriteRecord(record);
				}
			}

			using (var testDb = new TestDb(SqlBulkCopyWriterTests.DbConnectionString))
			{
				var recordsInDb = testDb.Delete("dbo.Person", records.ToArray());

				Assert.AreEqual(0, recordsInDb.Length);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void SqlBulkCopyWriter_ColumnMappingProviderThrowsException_ExceptionIsPropogated()
		{
			var record = Person.Faker.Generate(1).Single();
			var columnMappingProvider = MockRepository.GenerateMock<ISqlBulkCopyColumnMappingProvider>();

			columnMappingProvider.Stub(x => x.GetColumnMappings()).Throw(new InternalTestFailureException());

			using (var sqlBulkCopyWriter = new SqlBulkCopyWriter(columnMappingProvider, SqlBulkCopyWriterTests.DbConnectionString, "dbo.Person"))
			{
				sqlBulkCopyWriter.Open();
				sqlBulkCopyWriter.WriteRecord(record);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SqlBulkCopyWriter_InvalidColumnMappings_ExceptionIsPropogatedAndThrownFromCommit()
		{
			var invalidColumnMappings = new List<SqlBulkCopyColumnMapping>()
			{
				new SqlBulkCopyColumnMapping(nameof(Person.FirstName), "FirstNameINVALID"),
				new SqlBulkCopyColumnMapping(nameof(Person.LastName), "LastNameINVALID"),
				new SqlBulkCopyColumnMapping(nameof(Person.MiddleInitial), "MiddleInitialINVALID"),
				new SqlBulkCopyColumnMapping(nameof(Person.Age), "AgeINVALID"),
				new SqlBulkCopyColumnMapping(nameof(Person.DateOfBirth), "DateOfBirthINVALID"),
				new SqlBulkCopyColumnMapping(nameof(Person.Gender), "GenderINVALID")
			};

			var record = Person.Faker.Generate(1).Single();
			var columnMappingProvider = MockRepository.GenerateMock<ISqlBulkCopyColumnMappingProvider>();

			columnMappingProvider.Stub(x => x.GetColumnMappings()).Return(invalidColumnMappings);

			using (var sqlBulkCopyWriter = new SqlBulkCopyWriter(columnMappingProvider, SqlBulkCopyWriterTests.DbConnectionString, "dbo.Person"))
			{
				sqlBulkCopyWriter.Open();
				sqlBulkCopyWriter.WriteRecord(record);

				try
				{
					sqlBulkCopyWriter.Commit();
				}
				catch (AggregateException ae)
				{
					throw ae.InnerException;
				}
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SqlBulkCopyWriter_InvalidDestinationTable_RecordsAreWrittenToDatabase()
		{
			var records = Person.Faker.Generate(10);
			var columnMappingProvider = MockRepository.GenerateMock<ISqlBulkCopyColumnMappingProvider>();

			columnMappingProvider.Stub(x => x.GetColumnMappings()).Return(SqlBulkCopyWriterTests.ColumnMappings);

			using (var sqlBulkCopyWriter = new SqlBulkCopyWriter(columnMappingProvider, SqlBulkCopyWriterTests.DbConnectionString, "dbo.PersonINVALID"))
			{
				sqlBulkCopyWriter.Open();

				foreach (var record in records)
				{
					sqlBulkCopyWriter.WriteRecord(record);
				}

				try
				{
					sqlBulkCopyWriter.Commit();
				}
				catch (AggregateException ae)
				{
					throw ae.InnerException;
				}
			}
		}
	}
}
