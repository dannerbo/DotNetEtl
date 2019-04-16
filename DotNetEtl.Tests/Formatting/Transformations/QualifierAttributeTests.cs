using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Formatting.Transformations.Tests
{
	[TestClass]
	public class QualifierAttributeTests
	{
		[TestMethod]
		public void Transform_NonNullOrEmptyString_QualifiedValueReturned()
		{
			var value = "Test";
			var qualifier = "\"";
			var qualifierAttribute = new QualifierAttribute(qualifier, conditionalContains: null);
			var transformedValue = qualifierAttribute.Transform(value);

			Assert.AreEqual($"{qualifier}{value}{qualifier}", transformedValue);
		}

		[TestMethod]
		public void Transform_EmptyString_JustQualifiersReturned()
		{
			var value = String.Empty;
			var qualifier = "\"";
			var qualifierAttribute = new QualifierAttribute(qualifier, conditionalContains: null);
			var transformedValue = qualifierAttribute.Transform(value);

			Assert.AreEqual($"{qualifier}{qualifier}", transformedValue);
		}

		[TestMethod]
		public void Transform_NullValue_JustQualifiersReturned()
		{
			var value = (string)null;
			var qualifier = "\"";
			var qualifierAttribute = new QualifierAttribute(qualifier, conditionalContains: null);
			var transformedValue = qualifierAttribute.Transform(value);

			Assert.AreEqual($"{qualifier}{qualifier}", transformedValue);
		}

		[TestMethod]
		public void Transform_ConditionalQualifierWithValueMeetingCondition_QualifiedValueReturned()
		{
			var value = "Te,st";
			var qualifier = "\"";
			var qualifierAttribute = new QualifierAttribute(qualifier, conditionalContains: ",");
			var transformedValue = qualifierAttribute.Transform(value);

			Assert.AreEqual($"{qualifier}{value}{qualifier}", transformedValue);
		}

		[TestMethod]
		public void Transform_ConditionalQualifierWithValueNotMeetingCondition_OriginalValueReturned()
		{
			var value = "Test";
			var qualifier = "\"";
			var qualifierAttribute = new QualifierAttribute(qualifier, conditionalContains: ",");
			var transformedValue = qualifierAttribute.Transform(value);

			Assert.AreEqual(value, transformedValue);
		}
	}
}
