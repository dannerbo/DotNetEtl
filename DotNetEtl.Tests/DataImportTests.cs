using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportTests
	{
		[TestMethod]
		public void Constructor1_DataSourceIsProvided_DataSourcePropertyIsSet()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();

			var dataImport = new DataImport(dataSource, dataDestination);

			Assert.AreEqual(dataSource, dataImport.DataSource);
		}

		[TestMethod]
		public void Constructor2_DataSourceIsProvided_DataSourcePropertyIsSet()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataDestinations = new List<IDataDestination>() { dataDestination };

			var dataImport = new DataImport(dataSource, dataDestinations);

			Assert.AreEqual(dataSource, dataImport.DataSource);
		}

		[TestMethod]
		public void CanCommitWithRecordFailures_SetPropertyToTrue_PropertyIsSet()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();

			var dataImport = new DataImport(dataSource, dataDestination);

			Assert.AreEqual(dataSource, dataImport.DataSource);

			dataImport.CanCommitWithRecordFailures = true;

			Assert.IsTrue(dataImport.CanCommitWithRecordFailures);
		}

		[TestMethod]
		public void CanCommitWithRecordFailures_SetPropertyToFalse_PropertyIsSet()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.CanCommitWithRecordFailures = false;

			Assert.IsFalse(dataImport.CanCommitWithRecordFailures);
		}

		[TestMethod]
		public void TryRun_ZeroRecords_ImportSucceedsWithZeroFailures()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(false).Repeat.Once();
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsTrue(couldRun);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(0, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_OneReadableRecord_ImportSucceedsWithZeroFailures()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object()
			};
			var recordCounter = 0;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = null;

					mi.ReturnValue = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsTrue(couldRun);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_TwoReadableRecords_ImportSucceedsWithZeroFailures()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = null;

					mi.ReturnValue = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Once();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsTrue(couldRun);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(2, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(2, recordsWrittenCount);
		}
		
		[TestMethod]
		public void TryRun_OneUnreadableRecord_ImportFails()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(0, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsFalse(couldRun);
			Assert.AreEqual(readRecordFailures.Count, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(0, successfulRecordsReadCount);
			Assert.AreEqual(1, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_TwoUnreadableRecords_ImportFails()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(0, new List<FieldFailure>()
				{
					new FieldFailure()
				}),

				new RecordFailure(1, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Never();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsFalse(couldRun);
			Assert.AreEqual(readRecordFailures.Count, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(0, successfulRecordsReadCount);
			Assert.AreEqual(2, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_OneReadableAndOneUnreadableRecord_ImportFails()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(1, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsFalse(couldRun);
			Assert.AreEqual(readRecordFailures.Count, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(1, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}
		
		[TestMethod]
		public void TryRun_OneReadableAndOneUnreadableRecordWithCanCommitWithFailures_ImportSucceeds()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(1, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.CanCommitWithRecordFailures = true;

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsTrue(couldRun);
			Assert.AreEqual(readRecordFailures.Count, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(1, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_OneReadableRecordWithMultipleDataDestinations_ImportSucceedsWithZeroFailures()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestinations = new List<IDataDestination>()
			{
				dataDestination1,
				dataDestination2
			};
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object()
			};
			var recordCounter = 0;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = null;

					mi.ReturnValue = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination1.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter1).Repeat.Once();
			dataDestination2.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter2).Repeat.Once();
			dataWriter1.Expect(x => x.Open()).Repeat.Once();
			dataWriter1.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter1.Expect(x => x.Commit()).Repeat.Once();
			dataWriter1.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter1.Expect(x => x.Dispose()).Repeat.Once();
			dataWriter2.Expect(x => x.Open()).Repeat.Once();
			dataWriter2.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter2.Expect(x => x.Commit()).Repeat.Once();
			dataWriter2.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter2.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestinations);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination1.VerifyAllExpectations();
			dataDestination2.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();

			Assert.IsTrue(couldRun);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_ReadableRecordWithRecordMapper_RecordIsMapped()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var record = new object();
			var mappedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(mappedRecord))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);
			
			recordMapper.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(1, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_UnreadableRecordWithRecordMapper_RecordIsNotMapped()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var record = new object();
			var fieldFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = null;
					mi.Arguments[1] = fieldFailures;

					mi.ReturnValue = false;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordMapper.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(0, successfulRecordsReadCount);
			Assert.AreEqual(1, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_UnmappableRecord_ImportFails()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var record = new object();
			var mapRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(mapRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataReader.VerifyAllExpectations();
			recordMapper.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsFalse(couldRun);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(1, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_ReadableRecordWithRecordValidator_RecordIsValidated()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(record))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordValidator.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(1, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_UnreadableRecordWithRecordValidator_RecordIsNotValidated()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var fieldFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = null;
					mi.Arguments[1] = fieldFailures;

					mi.ReturnValue = false;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(record))).Repeat.Never();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordValidator.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(0, successfulRecordsReadCount);
			Assert.AreEqual(1, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_MappableRecordWithRecordValidator_MappedRecordIsValidated()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var mappedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(mappedRecord))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordValidator.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(1, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(1, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_UnmappableRecordWithRecordValidator_RecordIsNotValidated()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var mapRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(mapRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Anything,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordValidator.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(1, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_InvalidRecord_ImportFails()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var validateRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(validateRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataReader.VerifyAllExpectations();
			recordValidator.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsFalse(couldRun);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(1, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_ReadableRecordWithRecordFormatter_RecordIsFormatted()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(record))).Return(formattedRecord).Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(formattedRecord))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordFormatter.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(1, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_UnreadableRecordWithRecordFormatter_RecordIsNotFormatted()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var fieldFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = null;
					mi.Arguments[1] = fieldFailures;

					mi.ReturnValue = false;

					readRecordFlag = true;
				})
				.Return(false);
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Anything)).Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordFormatter.VerifyAllExpectations();

			Assert.AreEqual(0, successfulRecordsReadCount);
			Assert.AreEqual(1, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_UnmappableRecordWithRecordFormatter_RecordIsNotFormatted()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mapRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(mapRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Anything)).Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordFormatter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(1, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_InvalidRecordWithRecordFormatter_RecordIsNotFormatted()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var validateRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(validateRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Anything)).Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.TryRun(out var failures);

			recordFormatter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(1, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void TryRun_WritableRecordWithAllDependenciesProvided_ImportSucceeds()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(formattedRecord))).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();
			recordMapper.VerifyAllExpectations();
			recordValidator.VerifyAllExpectations();
			recordFormatter.VerifyAllExpectations();

			Assert.IsTrue(couldRun);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(1, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(1, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(1, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		[ExpectedException(typeof(OperationCanceledException))]
		public void TryRun_Cancellation_RollbackOccursAndExceptionIsThrown()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);
			
			using (var cancellationTokenSource = new CancellationTokenSource())
			{
				cancellationTokenSource.Cancel();

				try
				{
					dataImport.TryRun(cancellationTokenSource.Token, out var failures);
				}
				catch
				{
					dataWriter.VerifyAllExpectations();

					throw;
				}
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_DataDestinationCreateDataWriterThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Throw(new InternalTestFailureException());

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			dataImport.TryRun(out var failures);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_DataSourceCreateDateReaderThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataSource.Stub(x => x.CreateDataReader()).Throw(new InternalTestFailureException());
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_DataReaderOpenThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.Open()).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_DataReaderTryReadRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_RecordMapperTryMapRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_RecordValidatorTryValidateRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_RecordFormatterTryFormatRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		public void TryRun_DataWriterWriteRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Stub(x => x.WriteRecord(Arg<object>.Is.Equal(formattedRecord))).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (AggregateException ae)
			{
				Assert.IsInstanceOfType(ae.InnerException, typeof(InternalTestFailureException));

				dataWriter.VerifyAllExpectations();

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_DataWriterCommitThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Stub(x => x.Commit()).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();
			
			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_DataReaderDisposeThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			dataReader.Stub(x => x.Dispose()).Throw(new InternalTestFailureException());
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.TryRun(out var failures);
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryRun_DataWriterDisposeThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Stub(x => x.Dispose()).Throw(new InternalTestFailureException());

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);
			
			dataImport.TryRun(out var failures);
		}
		
		[TestMethod]
		public void Run_ZeroRecords_ImportSucceeds()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(false).Repeat.Once();
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();
			
			Assert.AreEqual(0, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(0, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_OneReadableRecord_ImportSucceedsWithZeroFailures()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object()
			};
			var recordCounter = 0;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = null;

					mi.ReturnValue = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			var couldRun = dataImport.TryRun(out var failures);

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.IsTrue(couldRun);
			Assert.AreEqual(0, failures.Count());
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_TwoReadableRecords_ImportSucceedsAndNoExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = null;

					mi.ReturnValue = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Once();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();
			
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(2, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(2, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_OneUnreadableRecord_ImportFailsAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(0, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException dataImportFailedException)
			{
				dataSource.VerifyAllExpectations();
				dataDestination.VerifyAllExpectations();
				dataReader.VerifyAllExpectations();
				dataWriter.VerifyAllExpectations();
				
				Assert.AreEqual(readRecordFailures.Count, dataImportFailedException.Failures.Count());
				Assert.AreEqual(records.Count, recordCounter);
				Assert.AreEqual(0, successfulRecordsReadCount);
				Assert.AreEqual(1, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_TwoUnreadableRecords_ImportFailsAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(0, new List<FieldFailure>()
				{
					new FieldFailure()
				}),

				new RecordFailure(1, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Never();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException dataImportFailedException)
			{
				dataSource.VerifyAllExpectations();
				dataDestination.VerifyAllExpectations();
				dataReader.VerifyAllExpectations();
				dataWriter.VerifyAllExpectations();
				
				Assert.AreEqual(readRecordFailures.Count, dataImportFailedException.Failures.Count());
				Assert.AreEqual(records.Count, recordCounter);
				Assert.AreEqual(0, successfulRecordsReadCount);
				Assert.AreEqual(2, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_OneReadableAndOneUnreadableRecord_ImportFailsAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(1, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException dataImportFailedException)
			{
				dataSource.VerifyAllExpectations();
				dataDestination.VerifyAllExpectations();
				dataReader.VerifyAllExpectations();
				dataWriter.VerifyAllExpectations();
				
				Assert.AreEqual(readRecordFailures.Count, dataImportFailedException.Failures.Count());
				Assert.AreEqual(records.Count, recordCounter);
				Assert.AreEqual(1, successfulRecordsReadCount);
				Assert.AreEqual(1, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(1, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_OneReadableAndOneUnreadableRecordWithCanCommitWithFailures_ImportSucceeds()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object(),
				new object()
			};
			var recordCounter = 0;
			var readRecordFailures = new List<RecordFailure>()
			{
				new RecordFailure(1, new List<FieldFailure>()
				{
					new FieldFailure()
				})
			};

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					var recordFailure = readRecordFailures.SingleOrDefault(x => x.RecordIndex == recordCounter);

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = recordFailure?.FieldFailures;

					mi.ReturnValue = recordFailure == null;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[1]))).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.CanCommitWithRecordFailures = true;

			dataImport.Run();

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(1, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_OneReadableRecordWithMultipleDataDestinations_ImportSucceeds()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination1 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestination2 = MockRepository.GenerateMock<IDataDestination>();
			var dataDestinations = new List<IDataDestination>()
			{
				dataDestination1,
				dataDestination2
			};
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter1 = MockRepository.GenerateMock<IDataWriter>();
			var dataWriter2 = MockRepository.GenerateMock<IDataWriter>();
			var records = new List<object>()
			{
				new object()
			};
			var recordCounter = 0;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (recordCounter >= records.Count)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = records[recordCounter++];
					mi.Arguments[1] = null;

					mi.ReturnValue = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination1.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter1).Repeat.Once();
			dataDestination2.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter2).Repeat.Once();
			dataWriter1.Expect(x => x.Open()).Repeat.Once();
			dataWriter1.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter1.Expect(x => x.Commit()).Repeat.Once();
			dataWriter1.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter1.Expect(x => x.Dispose()).Repeat.Once();
			dataWriter2.Expect(x => x.Open()).Repeat.Once();
			dataWriter2.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(records[0]))).Repeat.Once();
			dataWriter2.Expect(x => x.Commit()).Repeat.Once();
			dataWriter2.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter2.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestinations);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			dataSource.VerifyAllExpectations();
			dataDestination1.VerifyAllExpectations();
			dataDestination2.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter1.VerifyAllExpectations();
			dataWriter2.VerifyAllExpectations();
			
			Assert.AreEqual(records.Count, recordCounter);
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_ReadableRecordWithRecordMapper_RecordIsMapped()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var record = new object();
			var mappedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(mappedRecord))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			recordMapper.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(1, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_UnreadableRecordWithRecordMapper_RecordIsNotMappedAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var record = new object();
			var fieldFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = null;
					mi.Arguments[1] = fieldFailures;

					mi.ReturnValue = false;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				recordMapper.VerifyAllExpectations();
				dataWriter.VerifyAllExpectations();

				Assert.AreEqual(0, successfulRecordsReadCount);
				Assert.AreEqual(1, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_UnmappableRecord_ImportFailsAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var record = new object();
			var mapRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(mapRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				dataReader.VerifyAllExpectations();
				recordMapper.VerifyAllExpectations();
				dataWriter.VerifyAllExpectations();
				
				Assert.AreEqual(1, successfulRecordsReadCount);
				Assert.AreEqual(0, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(1, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_ReadableRecordWithRecordValidator_RecordIsValidated()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(record))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			recordValidator.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(1, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_UnreadableRecordWithRecordValidator_RecordIsNotValidatedAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var fieldFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = null;
					mi.Arguments[1] = fieldFailures;

					mi.ReturnValue = false;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(record))).Repeat.Never();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				recordValidator.VerifyAllExpectations();
				dataWriter.VerifyAllExpectations();

				Assert.AreEqual(0, successfulRecordsReadCount);
				Assert.AreEqual(1, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_MappableRecordWithRecordValidator_MappedRecordIsValidated()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var mappedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(mappedRecord))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			recordValidator.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(1, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(1, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(0, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_UnmappableRecordWithRecordValidator_RecordIsNotValidatedAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var mapRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(mapRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Anything,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator, recordMapper: recordMapper);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				recordValidator.VerifyAllExpectations();

				Assert.AreEqual(1, successfulRecordsReadCount);
				Assert.AreEqual(0, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(1, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_InvalidRecord_ImportFailsAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var record = new object();
			var validateRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(validateRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Anything)).Repeat.Never();
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				dataReader.VerifyAllExpectations();
				recordValidator.VerifyAllExpectations();
				dataWriter.VerifyAllExpectations();
				
				Assert.AreEqual(1, successfulRecordsReadCount);
				Assert.AreEqual(0, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(1, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_ReadableRecordWithRecordFormatter_RecordIsFormatted()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(record))).Return(formattedRecord).Repeat.Once();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(formattedRecord))).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			recordFormatter.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();

			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(0, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(0, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(1, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		public void Run_UnreadableRecordWithRecordFormatter_RecordIsNotFormattedAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var fieldFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = null;
					mi.Arguments[1] = fieldFailures;

					mi.ReturnValue = false;

					readRecordFlag = true;
				})
				.Return(false);
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Anything)).Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				recordFormatter.VerifyAllExpectations();

				Assert.AreEqual(0, successfulRecordsReadCount);
				Assert.AreEqual(1, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_UnmappableRecordWithRecordFormatter_RecordIsNotFormattedAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mapRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(mapRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Anything)).Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordMapper: recordMapper, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				recordFormatter.VerifyAllExpectations();

				Assert.AreEqual(1, successfulRecordsReadCount);
				Assert.AreEqual(0, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(1, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_InvalidRecordWithRecordFormatter_RecordIsNotFormattedAndExceptionIsThrown()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var validateRecordFailures = new List<FieldFailure>()
			{
				new FieldFailure()
			};
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(record),
					out Arg<IEnumerable<FieldFailure>>.Out(validateRecordFailures).Dummy))
				.Return(false)
				.Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Anything)).Repeat.Never();
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator: recordValidator, recordFormatter: recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			try
			{
				dataImport.Run();
			}
			catch (DataImportFailedException)
			{
				recordFormatter.VerifyAllExpectations();

				Assert.AreEqual(1, successfulRecordsReadCount);
				Assert.AreEqual(0, unsuccessfulRecordsReadCount);
				Assert.AreEqual(0, successfulRecordsMappedCount);
				Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
				Assert.AreEqual(0, successfulRecordsValidatedCount);
				Assert.AreEqual(1, unsuccessfulRecordsValidatedCount);
				Assert.AreEqual(0, recordsFormattedCount);
				Assert.AreEqual(0, recordsWrittenCount);

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		public void Run_WritableRecordWithAllDependenciesProvided_ImportSucceeds()
		{
			var successfulRecordsReadCount = 0;
			var unsuccessfulRecordsReadCount = 0;
			var successfulRecordsMappedCount = 0;
			var unsuccessfulRecordsMappedCount = 0;
			var successfulRecordsValidatedCount = 0;
			var unsuccessfulRecordsValidatedCount = 0;
			var recordsFormattedCount = 0;
			var recordsWrittenCount = 0;

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Expect(x => x.CreateDataReader()).Return(dataReader).Repeat.Once();
			dataReader.Expect(x => x.Open()).Repeat.Once();
			dataReader.Expect(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			dataReader.Expect(x => x.Dispose()).Repeat.Once();
			recordMapper.Expect(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordValidator.Expect(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true)
				.Repeat.Once();
			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord).Repeat.Once();
			dataDestination.Expect(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter).Repeat.Once();
			dataWriter.Expect(x => x.Open()).Repeat.Once();
			dataWriter.Expect(x => x.WriteRecord(Arg<object>.Is.Equal(formattedRecord))).Repeat.Once();
			dataWriter.Expect(x => x.Rollback()).Repeat.Never();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			dataImport.RecordRead += (sender, e) => { if (e.WasSuccessful) successfulRecordsReadCount++; else unsuccessfulRecordsReadCount++; };
			dataImport.RecordValidated += (sender, e) => { if (e.WasSuccessful) successfulRecordsValidatedCount++; else unsuccessfulRecordsValidatedCount++; };
			dataImport.RecordMapped += (sender, e) => { if (e.WasSuccessful) successfulRecordsMappedCount++; else unsuccessfulRecordsMappedCount++; };
			dataImport.RecordFormatted += (sender, e) => { recordsFormattedCount++; };
			dataImport.RecordWritten += (sender, e) => { recordsWrittenCount++; };

			dataImport.Run();

			dataSource.VerifyAllExpectations();
			dataDestination.VerifyAllExpectations();
			dataReader.VerifyAllExpectations();
			dataWriter.VerifyAllExpectations();
			recordMapper.VerifyAllExpectations();
			recordValidator.VerifyAllExpectations();
			recordFormatter.VerifyAllExpectations();
			
			Assert.AreEqual(1, successfulRecordsReadCount);
			Assert.AreEqual(0, unsuccessfulRecordsReadCount);
			Assert.AreEqual(1, successfulRecordsMappedCount);
			Assert.AreEqual(0, unsuccessfulRecordsMappedCount);
			Assert.AreEqual(1, successfulRecordsValidatedCount);
			Assert.AreEqual(0, unsuccessfulRecordsValidatedCount);
			Assert.AreEqual(1, recordsFormattedCount);
			Assert.AreEqual(1, recordsWrittenCount);
		}

		[TestMethod]
		[ExpectedException(typeof(OperationCanceledException))]
		public void Run_Cancellation_RollbackOccursAndExceptionIsThrown()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.Commit()).Repeat.Never();
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination);

			using (var cancellationTokenSource = new CancellationTokenSource())
			{
				cancellationTokenSource.Cancel();

				try
				{
					dataImport.Run(cancellationTokenSource.Token);
				}
				catch
				{
					dataWriter.VerifyAllExpectations();

					throw;
				}
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_DataDestinationCreateDataWriterThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Throw(new InternalTestFailureException());

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			dataImport.Run();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_DataSourceCreateDateReaderThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataSource.Stub(x => x.CreateDataReader()).Throw(new InternalTestFailureException());
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_DataReaderOpenThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.Open()).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_DataReaderTryReadRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_RecordMapperTryMapRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_RecordValidatorTryValidateRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_RecordFormatterTryFormatRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		public void Run_DataWriterWriteRecordThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(record).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy)).Return(true);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Stub(x => x.WriteRecord(Arg<object>.Is.Equal(formattedRecord))).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (AggregateException ae)
			{
				Assert.IsInstanceOfType(ae.InnerException, typeof(InternalTestFailureException));

				dataWriter.VerifyAllExpectations();

				return;
			}

			Assert.Fail();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_DataWriterCommitThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Stub(x => x.Commit()).Throw(new InternalTestFailureException());
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_DataReaderDisposeThrowsException_RollbackOccursAndExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			dataReader.Stub(x => x.Dispose()).Throw(new InternalTestFailureException());
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Expect(x => x.Rollback()).Repeat.Once();
			dataWriter.Expect(x => x.Dispose()).Repeat.Once();

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			try
			{
				dataImport.Run();
			}
			catch (InternalTestFailureException)
			{
				dataWriter.VerifyAllExpectations();

				throw;
			}
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Run_DataWriterDisposeThrowsException_ExceptionIsPropogated()
		{
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataDestination = MockRepository.GenerateMock<IDataDestination>();
			var dataReader = MockRepository.GenerateMock<IDataReader>();
			var dataWriter = MockRepository.GenerateMock<IDataWriter>();
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();
			var recordValidator = MockRepository.GenerateMock<IRecordValidator>();
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();
			var record = new object();
			var mappedRecord = new object();
			var formattedRecord = new object();
			var readRecordFlag = false;

			dataSource.Stub(x => x.CreateDataReader()).Return(dataReader);
			dataDestination.Stub(x => x.CreateDataWriter(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataWriter);
			dataReader.Stub(x => x.TryReadRecord(out Arg<object>.Out(null).Dummy, out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.WhenCalled(mi =>
				{
					if (readRecordFlag)
					{
						mi.Arguments[0] = null;
						mi.Arguments[1] = null;

						mi.ReturnValue = false;

						return;
					}

					mi.Arguments[0] = record;
					mi.Arguments[1] = null;

					mi.ReturnValue = true;

					readRecordFlag = true;
				})
				.Return(false);
			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(mappedRecord).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordValidator.Stub(x => x.TryValidate(
					Arg<object>.Is.Equal(mappedRecord),
					out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Return(true);
			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Equal(mappedRecord))).Return(formattedRecord);
			dataWriter.Stub(x => x.Dispose()).Throw(new InternalTestFailureException());

			var dataImport = new DataImport(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter);

			dataImport.Run();
		}
	}
}
