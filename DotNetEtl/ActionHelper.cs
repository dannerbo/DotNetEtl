using System;

namespace DotNetEtl
{
	public static class ActionHelper
	{
		public static void PerformCancelableAction(Action action)
		{
			try
			{
				action();
			}
			catch (OperationCanceledException)
			{
			}
			catch (AggregateException ae)
			{
				ae.Handle(ex => ex is OperationCanceledException);
			}
		}

		public static void PerformErrorableAction(Action action)
		{
			try
			{
				action();
			}
			catch
			{
			}
		}
	}
}
