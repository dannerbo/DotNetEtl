using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class DataAnnotatedRecordValidatorTests
	{
		[TestMethod]
		public void TryValidate_ObjectWithValidationAttributesAndValidValues_ValidationSucceedsWithZeroFailures()
		{
			var record = new MockRecord()
			{
				RequiredStringField = "Text",
				RangedIntField = 1
			};

			var dataAnnotatedRecordValidator = new DataAnnotatedRecordValidator();

			var couldValidate = dataAnnotatedRecordValidator.TryValidate(record, out var failures);

			Assert.IsTrue(couldValidate);
			Assert.AreEqual(0, failures.Count());
		}

		[TestMethod]
		public void TryValidate_ObjectWithValidationAttributesAndInvalidValues_ValidationFailsWithMultipleFailures()
		{
			var record = new MockRecord()
			{
				RequiredStringField = null,
				RangedIntField = 0
			};

			var dataAnnotatedRecordValidator = new DataAnnotatedRecordValidator();

			var couldValidate = dataAnnotatedRecordValidator.TryValidate(record, out var failures);

			Assert.IsFalse(couldValidate);
			Assert.AreEqual(2, failures.Count());
			Assert.IsNotNull(failures.SingleOrDefault(x => x.FieldName.Equals(nameof(MockRecord.RequiredStringField))));
			Assert.IsNotNull(failures.SingleOrDefault(x => x.FieldName.Equals(nameof(MockRecord.RangedIntField))));
		}

		[TestMethod]
		public void TryValidate_FieldDisplayNameProviderIsProvided_FailuresContainFieldNamesProvidedByFieldDisplayNameProvider()
		{
			var requiredStringFieldDisplayName = "RequiredStringField_Test";
			var rangedIntFieldDisplayName = "RangedIntField_Test";

			var record = new MockRecord()
			{
				RequiredStringField = null,
				RangedIntField = 0
			};

			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.RequiredStringField)))))
				.Return(requiredStringFieldDisplayName);
			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(
					Arg<PropertyInfo>.Matches(y => y.Name.Equals(nameof(MockRecord.RangedIntField)))))
				.Return(rangedIntFieldDisplayName);

			var dataAnnotatedRecordValidator = new DataAnnotatedRecordValidator(fieldDisplayNameProvider);

			var couldValidate = dataAnnotatedRecordValidator.TryValidate(record, out var failures);

			Assert.IsFalse(couldValidate);
			Assert.AreEqual(2, failures.Count());
			Assert.IsNotNull(failures.SingleOrDefault(x => x.FieldName.Equals(requiredStringFieldDisplayName)));
			Assert.IsNotNull(failures.SingleOrDefault(x => x.FieldName.Equals(rangedIntFieldDisplayName)));
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void TryValidate_FieldDisplayNameProviderThrowsException_ExceptionIsPropogated()
		{
			var record = new MockRecord()
			{
				RequiredStringField = null,
				RangedIntField = 0
			};

			var fieldDisplayNameProvider = MockRepository.GenerateMock<IFieldDisplayNameProvider>();

			fieldDisplayNameProvider.Stub(x => x.GetFieldDisplayName(Arg<PropertyInfo>.Is.Anything)).Throw(new InternalTestFailureException());

			var dataAnnotatedRecordValidator = new DataAnnotatedRecordValidator(fieldDisplayNameProvider);

			dataAnnotatedRecordValidator.TryValidate(record, out var failures);
		}

		[TestMethod]
		public void TryValidate_ObjectWithNoValidationAttributes_ValidationSucceedsWithZeroFailures()
		{
			var record = new MockRecordWithNoValidationAttributes();

			var dataAnnotatedRecordValidator = new DataAnnotatedRecordValidator();

			var couldValidate = dataAnnotatedRecordValidator.TryValidate(record, out var failures);

			Assert.IsTrue(couldValidate);
			Assert.AreEqual(0, failures.Count());
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void TryValidate_NullObjectProvided_ExceptionIsThrown()
		{
			var dataAnnotatedRecordValidator = new DataAnnotatedRecordValidator();

			dataAnnotatedRecordValidator.TryValidate(null, out var failures);
		}

		private class MockRecord
		{
			[Required]
			public string RequiredStringField { get; set; }
			[Range(1, 10)]
			public int? RangedIntField { get; set; }
		}

		private class MockRecordWithNoValidationAttributes
		{
			public string StringField { get; set; }
			public int IntField { get; set; }
		}
	}
}
