using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataSourceWatcherTests
	{
		[TestMethod]
		public void DataSourceWatcher_StartAndStop_NoExceptionIsThrown()
		{
			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.Start();
			dataSourceWatcher.Stop();
		}

		[TestMethod]
		public void DataSourceWatcher_StartAndStop_StateIsUpdatedAccordingly()
		{
			var startingStateFlag = false;
			var startedStateFlag = false;
			var stoppingStateFlag = false;
			var stoppedStateFlag = false;

			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
				startingStateFlag = dsw.State == DataSourceWatcherState.Starting;
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
				stoppingStateFlag = dsw.State == DataSourceWatcherState.Stopping;
			});

			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.Start();

			startedStateFlag = dataSourceWatcher.State == DataSourceWatcherState.Started;

			dataSourceWatcher.Stop();

			stoppedStateFlag = dataSourceWatcher.State == DataSourceWatcherState.Stopped;

			Assert.IsTrue(startingStateFlag);
			Assert.IsTrue(startedStateFlag);
			Assert.IsTrue(stoppingStateFlag);
			Assert.IsTrue(stoppedStateFlag);
		}
		
		[TestMethod]
		public void DataSourceWatcher_DataSourceIsAvailable_DataSourceAvailableEventIsFired()
		{
			var dataSourceAvailableEventFired = false;

			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.DataSourceAvailable += (sender, e) =>
			{
				dataSourceAvailableEventFired = true;

				Assert.AreEqual(dataSource, e.DataSource);
			};

			dataSourceWatcher.Start();
			dataSourceWatcher.RaiseDataSourceAvailableEvent(dataSource);
			dataSourceWatcher.Stop();

			Assert.IsTrue(dataSourceAvailableEventFired);
		}

		[TestMethod]
		public void DataSourceWatcher_ErrorOccurs_ErrorEventIsFired()
		{
			var exception = new InternalTestFailureException();
			var errorEventFired = false;

			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var dataSource = MockRepository.GenerateMock<IDataSource>();
			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.Error += (sender, e) =>
			{
				errorEventFired = true;

				Assert.AreEqual(exception, e.Exception);
			};

			dataSourceWatcher.Start();
			dataSourceWatcher.RaiseErrorEvent(exception);
			dataSourceWatcher.Stop();

			Assert.IsTrue(errorEventFired);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void DataSourceWatcher_OnStartThrowsException_ExceptionIsPropogated()
		{
			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
				throw new InternalTestFailureException();
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.Start();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void DataSourceWatcher_OnStopThrowsException_ExceptionIsPropogated()
		{
			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
				throw new InternalTestFailureException();
			});

			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.Start();
			dataSourceWatcher.Stop();
		}
		
		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSourceWatcher_StartTwiceWithoutStop_ExceptionIsThrown()
		{
			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.Start();
			dataSourceWatcher.Start();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void DataSourceWatcher_StopWithoutStart_ExceptionIsThrown()
		{
			var onStart = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var onStop = new Action<IDataSourceWatcher>(dsw =>
			{
			});

			var dataSourceWatcher = new MockDataSourceWatcher(onStart, onStop);

			dataSourceWatcher.Stop();
		}

		private class MockDataSourceWatcher : DataSourceWatcher
		{
			private Action<IDataSourceWatcher> onStart;
			private Action<IDataSourceWatcher> onStop;

			public MockDataSourceWatcher(Action<IDataSourceWatcher> onStart, Action<IDataSourceWatcher> onStop)
			{
				this.onStart = onStart;
				this.onStop = onStop;
			}

			public void RaiseDataSourceAvailableEvent(IDataSource dataSource)
			{
				this.OnDataSourceAvailable(dataSource);
			}

			public void RaiseErrorEvent(Exception exception)
			{
				this.OnError(exception);
			}

			protected override void OnStart()
			{
				this.onStart(this);
			}

			protected override void OnStop()
			{
				this.onStop(this);
			}
		}
	}
}
