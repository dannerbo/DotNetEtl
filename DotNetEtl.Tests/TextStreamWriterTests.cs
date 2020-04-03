using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class TextStreamWriterTests
	{
		[TestMethod]
		public void WriteRecord_OneRecord_RecordIsWritten()
		{
			var records = new string[]
			{
				"Record 1"
			};

			using (var stream = new MemoryStream())
			using (var streamWriter = new TextStreamWriter(stream))
			{
				streamWriter.Open();

				foreach (var record in records)
				{
					streamWriter.WriteRecord(record);
				}

				streamWriter.Commit();

				TextStreamWriterTests.AssertStreamContentMatchesRecords(stream, records);
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

			using (var stream = new MemoryStream())
			using (var streamWriter = new TextStreamWriter(stream))
			{
				streamWriter.Open();

				foreach (var record in records)
				{
					streamWriter.WriteRecord(record);
				}

				streamWriter.Commit();

				TextStreamWriterTests.AssertStreamContentMatchesRecords(stream, records);
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

			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Expect(x => x.Format(Arg<object>.Is.Equal(records[0]))).Return(formattedRecords[0]).Repeat.Once();

			using (var stream = new MemoryStream())
			using (var streamWriter = new TextStreamWriter(stream, recordFormatter))
			{
				streamWriter.Open();

				foreach (var record in records)
				{
					streamWriter.WriteRecord(record);
				}

				streamWriter.Commit();

				TextStreamWriterTests.AssertStreamContentMatchesRecords(stream, formattedRecords);
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

			var recordFormatter = MockRepository.GenerateMock<IRecordFormatter>();

			recordFormatter.Stub(x => x.Format(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			using (var stream = new MemoryStream())
			using (var streamWriter = new TextStreamWriter(stream, recordFormatter))
			{
				streamWriter.Open();
				streamWriter.WriteRecord(records[0]);
			}
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
	}
}
