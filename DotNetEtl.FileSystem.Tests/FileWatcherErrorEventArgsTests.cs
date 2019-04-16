using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.FileSystem.Tests
{
	[TestClass]
	public class FileWatcherErrorEventArgsTests
	{
		[TestMethod]
		public void Constructor_ExceptionIsProvided_ExceptionPropertyIsSetAndFilePathPropertyIsNotSet()
		{
			var exception = new Exception();
			var fileWatcherErrorEventArgs = new FileWatcherErrorEventArgs(exception);

			Assert.AreEqual(exception, fileWatcherErrorEventArgs.Exception);
			Assert.IsNull(fileWatcherErrorEventArgs.FilePath);
		}

		[TestMethod]
		public void Constructor_ExceptionAndFilePathIsProvided_PropertiesAreSet()
		{
			var exception = new Exception();
			var filePath = @"C:\Temp\testfile.txt";
			var fileWatcherErrorEventArgs = new FileWatcherErrorEventArgs(exception, filePath);

			Assert.AreEqual(exception, fileWatcherErrorEventArgs.Exception);
			Assert.AreEqual(filePath, fileWatcherErrorEventArgs.FilePath);
		}
	}
}
