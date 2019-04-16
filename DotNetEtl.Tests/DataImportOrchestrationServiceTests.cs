using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportOrchestrationServiceTests
	{
		[TestMethod]
		public void DataImportOrchestrationService_NoDataSources_StartsAndStopsSuccessfully()
		{
			var errorEventCount = 0;
			var dataImportPreventedEventCount = 0;
			var createDataImportErrorEventCount = 0;
			var dataImportStartedEventCount = 0;
			var dataImportCompletedEventCount = 0;
			var dataImportCanceledEventCount = 0;
			var dataImportErrorEventCount = 0;

			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();

			dataSourceWatcher.Expect(x => x.Start()).Repeat.Once();
			dataSourceWatcher.Expect(x => x.DataSourceAvailable += Arg<EventHandler<DataSourceEventArgs>>.Is.Anything).Repeat.Once();
			dataSourceWatcher.Expect(x => x.Stop()).Repeat.Once();

			var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

			dataImportOrchestrationService.Error += (sender, e) => errorEventCount++;
			dataImportOrchestrationService.DataImportPrevented += (sender, e) => dataImportPreventedEventCount++;
			dataImportOrchestrationService.CreateDataImportError += (sender, e) => createDataImportErrorEventCount++;
			dataImportOrchestrationService.DataImportStarted += (sender, e) => dataImportStartedEventCount++;
			dataImportOrchestrationService.DataImportCompleted += (sender, e) => dataImportCompletedEventCount++;
			dataImportOrchestrationService.DataImportCanceled += (sender, e) => dataImportCanceledEventCount++;
			dataImportOrchestrationService.DataImportError += (sender, e) => dataImportErrorEventCount++;

			dataImportOrchestrationService.Start();
			dataImportOrchestrationService.Stop();

			dataSourceWatcher.VerifyAllExpectations();

			Assert.AreEqual(0, errorEventCount);
			Assert.AreEqual(0, dataImportPreventedEventCount);
			Assert.AreEqual(0, createDataImportErrorEventCount);
			Assert.AreEqual(0, dataImportStartedEventCount);
			Assert.AreEqual(0, dataImportCompletedEventCount);
			Assert.AreEqual(0, dataImportCanceledEventCount);
			Assert.AreEqual(0, dataImportErrorEventCount);
		}

		[TestMethod]
		public void DataImportOrchestrationService_StartAndStop_StateIsUpdatedAccordingly()
		{
			var startingStateFlag = false;
			var startedStateFlag = false;
			var stoppingStateFlag = false;
			var stoppedStateFlag = false;

			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

			dataSourceWatcher.Stub(x => x.Start()).WhenCalled(mi => startingStateFlag = dataImportOrchestrationService.State == DataImportOrchestrationServiceState.Starting);
			dataSourceWatcher.Stub(x => x.Stop()).WhenCalled(mi => stoppingStateFlag = dataImportOrchestrationService.State == DataImportOrchestrationServiceState.Stopping);

			dataImportOrchestrationService.Start();

			startedStateFlag = dataImportOrchestrationService.State == DataImportOrchestrationServiceState.Started;

			dataImportOrchestrationService.Stop();

			stoppedStateFlag = dataImportOrchestrationService.State == DataImportOrchestrationServiceState.Stopped;

			Assert.IsTrue(startingStateFlag);
			Assert.IsTrue(startedStateFlag);
			Assert.IsTrue(stoppingStateFlag);
			Assert.IsTrue(stoppedStateFlag);
		}

		[TestMethod]
		public void DataImportOrchestrationService_StopWithActiveDataImportThatWillCompleteAndUsingInternalQueue_StopWaitsForDataImportToComplete()
		{
			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			using (var dataImportStartedSignal = new ManualResetEventSlim(false))
			using (var dataImportCompletedSignal = new ManualResetEventSlim(false))
			{
				dataImportFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataImport);
				dataImport.Stub(x => x.TryRun(Arg<CancellationToken>.Is.Anything, out Arg<IEnumerable<RecordFailure>>.Out(null).Dummy))
					.WhenCalled(mi =>
					{
						dataImportStartedSignal.Set();

						Thread.Sleep(TimeSpan.FromSeconds(1));

						dataImportCompletedSignal.Set();

						mi.ReturnValue = true;
					})
					.Return(default(bool));

				var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

				dataImportOrchestrationService.UseInternalQueue = true;

				dataImportOrchestrationService.Start();
				dataSourceWatcher.Raise(x => x.DataSourceAvailable += null, dataSourceWatcher, new DataSourceEventArgs(dataSource));
				dataImportStartedSignal.Wait(TimeSpan.FromSeconds(1));
				dataImportOrchestrationService.Stop();

				Assert.IsTrue(dataImportCompletedSignal.IsSet);
			}
		}

		[TestMethod]
		public void DataImportOrchestrationService_StopWithActiveDataImportThatWillCompleteAndNotUsingInternalQueue_StopWaitsForDataImportToComplete()
		{
			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			using (var dataImportStartedSignal = new ManualResetEventSlim(false))
			using (var dataImportCompletedSignal = new ManualResetEventSlim(false))
			{
				dataImportFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataImport);
				dataImport.Stub(x => x.TryRun(Arg<CancellationToken>.Is.Anything, out Arg<IEnumerable<RecordFailure>>.Out(null).Dummy))
					.WhenCalled(mi =>
					{
						dataImportStartedSignal.Set();

						Thread.Sleep(TimeSpan.FromSeconds(1));

						dataImportCompletedSignal.Set();

						mi.ReturnValue = true;
					})
					.Return(default(bool));

				var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

				dataImportOrchestrationService.UseInternalQueue = false;

				dataImportOrchestrationService.Start();
				dataSourceWatcher.Raise(x => x.DataSourceAvailable += null, dataSourceWatcher, new DataSourceEventArgs(dataSource));
				dataImportStartedSignal.Wait(TimeSpan.FromSeconds(1));
				dataImportOrchestrationService.Stop();

				Assert.IsTrue(dataImportCompletedSignal.IsSet);
			}
		}

		[TestMethod]
		public void DataImportOrchestrationService_StopWithActiveDataImportThatWillCancelAndUsingInternalQueue_StopWaitsForDataImportToCancel()
		{
			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			using (var dataImportStartedSignal = new ManualResetEventSlim(false))
			using (var dataImportCanceledSignal = new ManualResetEventSlim(false))
			{
				dataImportFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataImport);
				dataImport.Stub(x => x.TryRun(Arg<CancellationToken>.Is.Anything, out Arg<IEnumerable<RecordFailure>>.Out(null).Dummy))
					.WhenCalled(mi =>
					{
						dataImportStartedSignal.Set();

						Thread.Sleep(TimeSpan.FromSeconds(1));

						dataImportCanceledSignal.Set();

						throw new OperationCanceledException();
					})
					.Return(default(bool));

				var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

				dataImportOrchestrationService.UseInternalQueue = true;

				dataImportOrchestrationService.Start();
				dataSourceWatcher.Raise(x => x.DataSourceAvailable += null, dataSourceWatcher, new DataSourceEventArgs(dataSource));
				dataImportStartedSignal.Wait(TimeSpan.FromSeconds(1));
				dataImportOrchestrationService.Stop();

				Assert.IsTrue(dataImportCanceledSignal.IsSet);
			}
		}

		[TestMethod]
		public void DataImportOrchestrationService_StopWithActiveDataImportThatWillCancelAndNotUsingInternalQueue_StopWaitsForDataImportToCancel()
		{
			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			using (var dataImportStartedSignal = new ManualResetEventSlim(false))
			using (var dataImportCanceledSignal = new ManualResetEventSlim(false))
			{
				dataImportFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataImport);
				dataImport.Stub(x => x.TryRun(Arg<CancellationToken>.Is.Anything, out Arg<IEnumerable<RecordFailure>>.Out(null).Dummy))
					.WhenCalled(mi =>
					{
						dataImportStartedSignal.Set();

						Thread.Sleep(TimeSpan.FromSeconds(1));

						dataImportCanceledSignal.Set();

						throw new OperationCanceledException();
					})
					.Return(default(bool));

				var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

				dataImportOrchestrationService.UseInternalQueue = false;

				dataImportOrchestrationService.Start();
				dataSourceWatcher.Raise(x => x.DataSourceAvailable += null, dataSourceWatcher, new DataSourceEventArgs(dataSource));
				dataImportStartedSignal.Wait(TimeSpan.FromSeconds(1));
				dataImportOrchestrationService.Stop();

				Assert.IsTrue(dataImportCanceledSignal.IsSet);
			}
		}

		[TestMethod]
		public void DataImportOrchestrationService_DataImportCompletesWithNoFailures_AllEventsFireAsExpected()
		{
			var errorEventCount = 0;
			var dataImportPreventedEventCount = 0;
			var createDataImportErrorEventCount = 0;
			var dataImportStartedEventCount = 0;
			var dataImportCompletedEventCount = 0;
			var dataImportCanceledEventCount = 0;
			var dataImportErrorEventCount = 0;

			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			using (var dataImportCompletedSignal = new ManualResetEventSlim(false))
			{
				dataImportFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataImport);
				dataImport.Stub(x => x.TryRun(Arg<CancellationToken>.Is.Anything, out Arg<IEnumerable<RecordFailure>>.Out(null).Dummy))
					.WhenCalled(mi =>
					{
						dataImportCompletedSignal.Set();

						mi.ReturnValue = true;
					})
					.Return(default(bool));

				var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

				dataImportOrchestrationService.Error += (sender, e) => errorEventCount++;
				dataImportOrchestrationService.DataImportPrevented += (sender, e) => dataImportPreventedEventCount++;
				dataImportOrchestrationService.CreateDataImportError += (sender, e) => createDataImportErrorEventCount++;
				dataImportOrchestrationService.DataImportStarted += (sender, e) => dataImportStartedEventCount++;
				dataImportOrchestrationService.DataImportCompleted += (sender, e) => dataImportCompletedEventCount++;
				dataImportOrchestrationService.DataImportCanceled += (sender, e) => dataImportCanceledEventCount++;
				dataImportOrchestrationService.DataImportError += (sender, e) => dataImportErrorEventCount++;
				
				dataImportOrchestrationService.Start();
				dataSourceWatcher.Raise(x => x.DataSourceAvailable += null, dataSourceWatcher, new DataSourceEventArgs(dataSource));
				dataImportCompletedSignal.Wait(TimeSpan.FromSeconds(1));
				dataImportOrchestrationService.Stop();
			}

			Assert.AreEqual(0, errorEventCount);
			Assert.AreEqual(0, dataImportPreventedEventCount);
			Assert.AreEqual(0, createDataImportErrorEventCount);
			Assert.AreEqual(1, dataImportStartedEventCount);
			Assert.AreEqual(1, dataImportCompletedEventCount);
			Assert.AreEqual(0, dataImportCanceledEventCount);
			Assert.AreEqual(0, dataImportErrorEventCount);
		}
		
		[TestMethod]
		public void DataImportOrchestrationService_DataImportThrowsException_AllEventsFireAsExpected()
		{
			var errorEventCount = 0;
			var dataImportPreventedEventCount = 0;
			var createDataImportErrorEventCount = 0;
			var dataImportStartedEventCount = 0;
			var dataImportCompletedEventCount = 0;
			var dataImportCanceledEventCount = 0;
			var dataImportErrorEventCount = 0;

			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			using (var canStopSignal = new ManualResetEventSlim(false))
			{
				dataImportFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataImport);
				dataImport.Stub(x => x.TryRun(Arg<CancellationToken>.Is.Anything, out Arg<IEnumerable<RecordFailure>>.Out(null).Dummy))
					.WhenCalled(mi =>
					{
						canStopSignal.Set();

						throw new InternalTestFailureException();
					})
					.Return(default(bool));

				var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

				dataImportOrchestrationService.Error += (sender, e) => errorEventCount++;
				dataImportOrchestrationService.DataImportPrevented += (sender, e) => dataImportPreventedEventCount++;
				dataImportOrchestrationService.CreateDataImportError += (sender, e) => createDataImportErrorEventCount++;
				dataImportOrchestrationService.DataImportStarted += (sender, e) => dataImportStartedEventCount++;
				dataImportOrchestrationService.DataImportCompleted += (sender, e) => dataImportCompletedEventCount++;
				dataImportOrchestrationService.DataImportCanceled += (sender, e) => dataImportCanceledEventCount++;
				dataImportOrchestrationService.DataImportError += (sender, e) => dataImportErrorEventCount++;

				dataImportOrchestrationService.Start();
				dataSourceWatcher.Raise(x => x.DataSourceAvailable += null, dataSourceWatcher, new DataSourceEventArgs(dataSource));
				canStopSignal.Wait(TimeSpan.FromSeconds(1));
				dataImportOrchestrationService.Stop();
			}

			Assert.AreEqual(0, errorEventCount);
			Assert.AreEqual(0, dataImportPreventedEventCount);
			Assert.AreEqual(0, createDataImportErrorEventCount);
			Assert.AreEqual(1, dataImportStartedEventCount);
			Assert.AreEqual(0, dataImportCompletedEventCount);
			Assert.AreEqual(0, dataImportCanceledEventCount);
			Assert.AreEqual(1, dataImportErrorEventCount);
		}

		[TestMethod]
		public void DataImportOrchestrationService_DataImportIsCanceled_AllEventsFireAsExpected()
		{
			var errorEventCount = 0;
			var dataImportPreventedEventCount = 0;
			var createDataImportErrorEventCount = 0;
			var dataImportStartedEventCount = 0;
			var dataImportCompletedEventCount = 0;
			var dataImportCanceledEventCount = 0;
			var dataImportErrorEventCount = 0;

			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();
			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataImport = MockRepository.GenerateMock<IDataImport>();

			using (var canStopSignal = new ManualResetEventSlim(false))
			{
				dataImportFactory.Stub(x => x.Create(Arg<IDataSource>.Is.Equal(dataSource))).Return(dataImport);
				dataImport.Stub(x => x.TryRun(Arg<CancellationToken>.Is.Anything, out Arg<IEnumerable<RecordFailure>>.Out(null).Dummy))
					.WhenCalled(mi =>
					{
						canStopSignal.Set();

						throw new OperationCanceledException();
					})
					.Return(default(bool));

				var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

				dataImportOrchestrationService.Error += (sender, e) => errorEventCount++;
				dataImportOrchestrationService.DataImportPrevented += (sender, e) => dataImportPreventedEventCount++;
				dataImportOrchestrationService.CreateDataImportError += (sender, e) => createDataImportErrorEventCount++;
				dataImportOrchestrationService.DataImportStarted += (sender, e) => dataImportStartedEventCount++;
				dataImportOrchestrationService.DataImportCompleted += (sender, e) => dataImportCompletedEventCount++;
				dataImportOrchestrationService.DataImportCanceled += (sender, e) => dataImportCanceledEventCount++;
				dataImportOrchestrationService.DataImportError += (sender, e) => dataImportErrorEventCount++;

				dataImportOrchestrationService.Start();
				dataSourceWatcher.Raise(x => x.DataSourceAvailable += null, dataSourceWatcher, new DataSourceEventArgs(dataSource));
				canStopSignal.Wait(TimeSpan.FromSeconds(1));
				dataImportOrchestrationService.Stop();
			}

			Assert.AreEqual(0, errorEventCount);
			Assert.AreEqual(0, dataImportPreventedEventCount);
			Assert.AreEqual(0, createDataImportErrorEventCount);
			Assert.AreEqual(1, dataImportStartedEventCount);
			Assert.AreEqual(0, dataImportCompletedEventCount);
			Assert.AreEqual(1, dataImportCanceledEventCount);
			Assert.AreEqual(0, dataImportErrorEventCount);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataImportOrchestrationService_StartTwiceWithoutStop_ExceptionIsThrown()
		{
			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();

			var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

			dataImportOrchestrationService.Start();
			dataImportOrchestrationService.Start();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataImportOrchestrationService_StopWithoutStart_ExceptionIsThrown()
		{
			var dataSourceWatcher = MockRepository.GenerateMock<IDataSourceWatcher>();
			var dataImportFactory = MockRepository.GenerateMock<IDataImportFactory>();

			var dataImportOrchestrationService = new DataImportOrchestrationService(dataSourceWatcher, dataImportFactory);

			dataImportOrchestrationService.Stop();
		}
	}
}
