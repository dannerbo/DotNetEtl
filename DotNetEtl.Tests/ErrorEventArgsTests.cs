using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class ErrorEventArgsTests
	{
		[TestMethod]
		public void Constructor_ExceptionIsProvided_ExceptionPropertiesIsSet()
		{
			var exception = new Exception();
			var errorEventArgs = new ErrorEventArgs(exception);

			Assert.AreEqual(exception, errorEventArgs.Exception);
		}
	}
}
