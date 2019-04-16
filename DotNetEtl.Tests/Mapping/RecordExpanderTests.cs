using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Mapping.Tests
{
	[TestClass]
	public class RecordExpanderTests
	{
		[TestMethod]
		public void Expand_RecordTypesProvided_RecordIsExpanded()
		{
			var record = new MockRecord()
			{
				StringField = "text",
				IntField = 1
			};

			var recordExpander = new RecordExpander(typeof(MockExpandedRecordA), typeof(MockExpandedRecordB));

			var expandedRecords = recordExpander.Expand(record);

			Assert.AreEqual(2, expandedRecords.Count());
			Assert.AreEqual(record.StringField, ((MockExpandedRecordA)expandedRecords.Single(x => x is MockExpandedRecordA)).StringField);
			Assert.AreEqual(record.StringField, ((MockExpandedRecordB)expandedRecords.Single(x => x is MockExpandedRecordB)).StringField);
			Assert.AreEqual(record.IntField, ((MockExpandedRecordB)expandedRecords.Single(x => x is MockExpandedRecordB)).IntField);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Expand_RecordMapperCannotMapRecord_ExceptionIsThrown()
		{
			var record = new MockRecord()
			{
				StringField = "text",
				IntField = 1
			};

			var failures = new List<FieldFailure>()
			{
				new FieldFailure()
				{
					FieldName = nameof(MockRecord.StringField),
					Message = "Field is invalid."
				}
			};

			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Equal(record),
					out Arg<object>.Out(null).Dummy,
					out Arg<IEnumerable<FieldFailure>>.Out(failures).Dummy))
				.Return(false);

			var recordExpander = new RecordExpander(recordMapper);

			recordExpander.Expand(record);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Expand_NullRecord_ExceptionIsThrown()
		{
			var recordExpander = new RecordExpander(typeof(MockExpandedRecordA), typeof(MockExpandedRecordB));

			recordExpander.Expand(null);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Expand_RecordMapperThrowsException_ExceptionIsPropogated()
		{
			var record = new MockRecord()
			{
				StringField = "text",
				IntField = 1
			};
			
			var recordMapper = MockRepository.GenerateMock<IRecordMapper>();

			recordMapper.Stub(x => x.TryMap(
					Arg<object>.Is.Anything,
						out Arg<object>.Out(null).Dummy,
						out Arg<IEnumerable<FieldFailure>>.Out(null).Dummy))
				.Throw(new InternalTestFailureException());

			var recordExpander = new RecordExpander(recordMapper);

			recordExpander.Expand(record);
		}

		private class MockRecord
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
		}

		private class MockExpandedRecordA
		{
			[SourceFieldName("StringField")]
			public string StringField { get; set; }
		}

		private class MockExpandedRecordB
		{
			[SourceFieldName("StringField")]
			public string StringField { get; set; }

			[SourceFieldName("IntField")]
			public int IntField { get; set; } 
		}
	}
}
