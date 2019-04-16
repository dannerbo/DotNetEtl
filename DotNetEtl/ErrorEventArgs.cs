using System;

namespace DotNetEtl
{
	public class ErrorEventArgs : EventArgs
	{
		public ErrorEventArgs(Exception exception)
		{
			this.Exception = exception;
		}

		public Exception Exception { get; private set; }
	}
}
