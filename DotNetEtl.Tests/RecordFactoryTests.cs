using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class RecordFactoryTests
	{
		[TestMethod]
		public void Create_RecordTypeProviderIsNotProvided_RecordIsCreated()
		{
			var source = new object();
			var record = new object();
			var createRecord = new Func<object, object, object>((src, rt) => src.Equals(source) ? record : null);

			var recordFactory = new RecordFactory(createRecord);

			var returnedRecord = recordFactory.Create(source);

			Assert.AreEqual(record, returnedRecord);
		}

		[TestMethod]
		public void Create_RecordTypeProviderIsProvided_RecordIsCreated()
		{
			var source = new object();
			var record = new object();
			var recordType = "RecordType";
			var createRecord = new Func<object, object, object>((src, rt) => src.Equals(source) && rt.Equals(recordType) ? record : null);
			var recordTypeProvider = MockRepository.GenerateMock<IRecordTypeProvider>();

			recordTypeProvider.Expect(x => x.GetRecordType(Arg<object>.Is.Equal(source))).Return(recordType).Repeat.Once();

			var recordFactory = new RecordFactory(createRecord, recordTypeProvider);

			var returnedRecord = recordFactory.Create(source);

			recordTypeProvider.VerifyAllExpectations();

			Assert.AreEqual(record, returnedRecord);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_CreateRecordFuncThrowsException_ExceptionIsPropogated()
		{
			var source = new object();
			var record = new object();
			var createRecord = new Func<object, object, object>((src, rt) => throw new InternalTestFailureException());

			var recordFactory = new RecordFactory(createRecord);

			recordFactory.Create(source);
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void Create_RecordTypeProviderThrowsException_ExceptionIsPropogated()
		{
			var source = new object();
			var record = new object();
			var recordType = "RecordType";
			var createRecord = new Func<object, object, object>((src, rt) => src.Equals(source) && rt.Equals(recordType) ? record : null);
			var recordTypeProvider = MockRepository.GenerateMock<IRecordTypeProvider>();

			recordTypeProvider.Stub(x => x.GetRecordType(Arg<object>.Is.Anything)).Throw(new InternalTestFailureException());

			var recordFactory = new RecordFactory(createRecord, recordTypeProvider);

			recordFactory.Create(source);
		}
	}
}
