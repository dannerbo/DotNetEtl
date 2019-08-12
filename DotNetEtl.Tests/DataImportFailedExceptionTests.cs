using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataImportFailedExceptionTests
	{
		[TestMethod]
		public void Constructor1_FailuresAreProvided_FailuresPropertyIsSet()
		{
			var failures = MockRepository.GenerateMock<IEnumerable<RecordFailure>>();

			var dataImportFailedException = new DataImportFailedException(failures);

			Assert.AreEqual(failures, dataImportFailedException.Failures);
		}

		[TestMethod]
		public void Constructor2_FailuresAndMessageAreProvided_FailuresAndMessagePropertiesAreSet()
		{
			var failures = MockRepository.GenerateMock<IEnumerable<RecordFailure>>();
			var message = "Test error message";

			var dataImportFailedException = new DataImportFailedException(failures, message);

			Assert.AreEqual(failures, dataImportFailedException.Failures);
			Assert.AreEqual(message, dataImportFailedException.Message);
		}

		[TestMethod]
		public void Constructor3_DataImportAndMessageAndInnerExceptionAreProvided_DataImportAndMessageAndInnerExceptionPropertiesAreSet()
		{
			var failures = MockRepository.GenerateMock<IEnumerable<RecordFailure>>();
			var message = "Test error message";
			var innerException = new Exception();

			var dataImportFailedException = new DataImportFailedException(failures, message, innerException);

			Assert.AreEqual(failures, dataImportFailedException.Failures);
			Assert.AreEqual(message, dataImportFailedException.Message);
			Assert.AreEqual(innerException, dataImportFailedException.InnerException);
		}
	}
}
