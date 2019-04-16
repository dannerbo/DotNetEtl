using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class ActionHelperTests
	{
		[TestMethod]
		public void PerformCancelableAction_ActionWithNoExceptionsThrown_NoExceptionIsThrown()
		{
			ActionHelper.PerformCancelableAction(() => { });
		}

		[TestMethod]
		public void PerformCancelableAction_OperationCanceledExceptionThrown_ExceptionIsNotPropogated()
		{
			ActionHelper.PerformCancelableAction(() => throw new OperationCanceledException());
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void PerformCancelableAction_UnexpectedExceptionIsThrown_ExceptionIsPropogated()
		{
			ActionHelper.PerformCancelableAction(() => throw new InternalTestFailureException());
		}

		[TestMethod]
		public void PerformCancelableAction_AggregateExceptionContainingOperationCanceledExceptionIsThrown_ExceptionIsNotPropogated()
		{
			var operationCanceledException = new OperationCanceledException();

			ActionHelper.PerformCancelableAction(() => throw new AggregateException(operationCanceledException));
		}

		[TestMethod]
		[ExpectedException(typeof(AggregateException))]
		public void PerformCancelableAction_AggregateExceptionContainingUnexpectedExceptionIsThrown_ExceptionIsPropogated()
		{
			var internalTestFailureException = new InternalTestFailureException();

			ActionHelper.PerformCancelableAction(() => throw new AggregateException(internalTestFailureException));
		}

		[TestMethod]
		[ExpectedException(typeof(AggregateException))]
		public void PerformCancelableAction_AggregateExceptionContainingUnexpectedExceptionAndOperationCanceledExceptionIsThrown_ExceptionIsPropogated()
		{
			var internalTestFailureException = new InternalTestFailureException();
			var operationCanceledException = new OperationCanceledException();

			ActionHelper.PerformCancelableAction(() => throw new AggregateException(internalTestFailureException, operationCanceledException));
		}

		[TestMethod]
		public void PerformErrorableAction_NoExceptionIsThrown_NoExceptionIsThrown()
		{
			ActionHelper.PerformErrorableAction(() => { });
		}

		[TestMethod]
		public void PerformErrorableAction_ExceptionIsThrown_ExceptionIsNotPropogated()
		{
			ActionHelper.PerformErrorableAction(() => throw new InternalTestFailureException());
		}
	}
}
