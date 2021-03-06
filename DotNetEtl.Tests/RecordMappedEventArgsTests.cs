﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordMappedEventArgsTests
	{
		[TestMethod]
		public void Constructor_RecordAndWasSuccessfulAndFailuresAndMappedRecordAreProvided_PropertiesAreSet()
		{
			var recordIndex = 1;
			var record = new object();
			var wasSuccessful = true;
			var failures = new FieldFailure[0];
			var mappedRecord = new object();

			var recordMappedEventArgs = new RecordMappedEventArgs(recordIndex, record, wasSuccessful, failures, mappedRecord);

			Assert.AreEqual(recordIndex, recordMappedEventArgs.RecordIndex);
			Assert.AreEqual(record, recordMappedEventArgs.Record);
			Assert.AreEqual(wasSuccessful, recordMappedEventArgs.WasSuccessful);
			Assert.AreEqual(failures, recordMappedEventArgs.Failures);
			Assert.AreEqual(mappedRecord, recordMappedEventArgs.MappedRecord);
		}
	}
}
