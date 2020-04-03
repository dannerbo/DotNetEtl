using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class StreamWriterTests
	{
		[TestMethod]
		public void WriteRecord_OneRecord_RecordIsWritten()
		{
			var records = new string[]
			{
				"Record 1"
			};

			var writeRecord = new Action<System.IO.StreamWriter, object>((sr, r) => sr.WriteLine(r));

			using (var stream = new MemoryStream())
			using (var streamWriter = new MockStreamWriter(stream, writeRecord))
			{
				streamWriter.Open();

				foreach (var record in records)
				{
					streamWriter.WriteRecord(record);
				}

				streamWriter.Commit();

				StreamWriterTests.AssertStreamContentMatchesRecords(stream, records);
			}
		}

		[TestMethod]
		public void WriteRecord_TwoRecords_RecordsAreWritten()
		{
			var records = new string[]
			{
				"Record 1",
				"Record 2"
			};

			var writeRecord = new Action<System.IO.StreamWriter, object>((sr, r) => sr.WriteLine(r));

			using (var stream = new MemoryStream())
			using (var streamWriter = new MockStreamWriter(stream, writeRecord))
			{
				streamWriter.Open();

				foreach (var record in records)
				{
					streamWriter.WriteRecord(record);
				}

				streamWriter.Commit();

				StreamWriterTests.AssertStreamContentMatchesRecords(stream, records);
			}
		}

		[TestMethod]
		public void WriteRecord_OneRecordWithRecordFormatter_RecordIsFormattedAndWritten()
		{
			var records = new string[]
			{
				"Record 1"
			};

			var formattedRecords = new string[]
			{
				"Formatted Record 1"
			};

			var writeRecord = new Action<System.IO.StreamWriter, object>((sr, r) => sr.WriteLine(r));
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(records[0]))).Return(formattedRecords[0]).Repeat.Once();

			using (var stream = new MemoryStream())
			using (var streamWriter = new MockStreamWriter(stream, writeRecord, recordFormatter))
			{
				streamWriter.Open();

				foreach (var record in records)
				{
					streamWriter.WriteRecord(record);
				}

				streamWriter.Commit();

				StreamWriterTests.AssertStreamContentMatchesRecords(stream, formattedRecords);
			}

			recordFormatter.VerifyAllExpectations();
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void WriteRecord_RecordFormatterThrowsException_ExceptionIsPropogated()
		{
			var records = new string[]
			{
				"Record 1"
			};

			var writeRecord = new Action<System.IO.StreamWriter, object>((sr, r) => sr.WriteLine(r));
			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			using (var stream = new MemoryStream())
			using (var streamWriter = new MockStreamWriter(stream, writeRecord, recordFormatter))
			{
				streamWriter.Open();
				streamWriter.WriteRecord(records[0]);
			}
		}

		[TestMethod]
		public void Dispose_Dispose_CloseIsCalled()
		{
			var closeWasCalled = false;
			var close = new Action(() => closeWasCalled = true);
			var streamWriter = new MockStreamWriter(close);

			streamWriter.Dispose();

			Assert.IsTrue(closeWasCalled);
		}

		private static void AssertStreamContentMatchesRecords(MemoryStream stream, string[] records)
		{
			var recordsStringBuilder = new StringBuilder();

			foreach (var record in records)
			{
				recordsStringBuilder.AppendLine(record);
			}

			stream.Seek(0, SeekOrigin.Begin);

			using (var stringReader = new System.IO.StreamReader(stream))
			{
				var expected = recordsStringBuilder.ToString();
				var actual = stringReader.ReadToEnd();

				Assert.AreEqual(expected, actual);
			}
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