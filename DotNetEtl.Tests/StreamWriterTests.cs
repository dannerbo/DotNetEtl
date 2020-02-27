using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class StreamWriterTests
	{
		[TestMethod]
		public void WriteRecord_OneRecord_RecordIsWritten()
		{
			var record = "Record 1";
			var recordWritten = (object)null;
			var writeRecord = new Action<System.IO.StreamWriter, object>((sr, r) =>
			{
				sr.WriteLine(r);
				recordWritten = r;
			});
			
			using (var stream = new MemoryStream())
			using (var streamWriter = new MockStreamWriter(stream, writeRecord))
			{
				streamWriter.Open();
				streamWriter.WriteRecord(record);
			}

			Assert.AreEqual(record, recordWritten);
		}

		[TestMethod]
		public void WriteRecord_TwoRecords_RecordsAreWritten()
		{
			var records = new string[]
			{
				"Record 1",
				"Record 2"
			};
			var recordsWritten = new List<string>();
			var writeRecord = new Action<System.IO.StreamWriter, object>((sr, r) =>
			{
				sr.WriteLine(r);
				recordsWritten.Add((string)r);
			});

			using (var stream = new MemoryStream())
			using (var streamWriter = new MockStreamWriter(stream, writeRecord))
			{
				streamWriter.Open();

				foreach (var record in records)
				{
					streamWriter.WriteRecord(record);
				}
			}

			CollectionAssert.AreEquivalent(records, recordsWritten);
		}

		private class MockStreamWriter : StreamWriter
		{
			private Action<System.IO.StreamWriter, object> writeRecord;
			private Action close;

			public MockStreamWriter(Stream stream, Action<System.IO.StreamWriter, object> writeRecord, IRecordFormatter recordFormatter = null)
				: base(stream, recordFormatter)
			{
				this.writeRecord = writeRecord;
			}

			public MockStreamWriter(IStreamFactory streamFactory, Action<System.IO.StreamWriter, object> writeRecord, IRecordFormatter recordFormatter = null)
				: base(streamFactory, recordFormatter)
			{
				this.writeRecord = writeRecord;
			}

			public MockStreamWriter(Action close)
				: base((Stream)null)
			{
				this.close = close;
			}

			public override void Commit()
			{
				throw new NotImplementedException();
			}

			public override void Rollback()
			{
				throw new NotImplementedException();
			}

			public override void Close()
			{
				this.close?.Invoke();

				base.Close();
			}

			protected override void WriteRecordInternal(object record)
			{
				this.writeRecord(this.InternalStreamWriter, record);
			}
		}
	}
}