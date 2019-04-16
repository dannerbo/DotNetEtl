using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNetEtl.Tests
{
	[TestClass]
	public class FieldDisplayNameProviderTests
	{
		[TestMethod]
		public void GetFieldDisplayName_PropertyWithAttribute_OverridenDisplayNameIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithDisplayAttribute));
			var fieldDisplayNameProvider = new FieldDisplayNameProvider();
			var fieldDisplayName = fieldDisplayNameProvider.GetFieldDisplayName(property);

			Assert.AreEqual("FieldWithDisplayAttributeX", fieldDisplayName);
		}

		[TestMethod]
		public void GetFieldDisplayName_PropertyWithoutAttribute_PropertyNameIsReturned()
		{
			var property = typeof(MockRecord).GetProperty(nameof(MockRecord.FieldWithoutDisplayAttribute));
			var fieldDisplayNameProvider = new FieldDisplayNameProvider();
			var fieldDisplayName = fieldDisplayNameProvider.GetFieldDisplayName(property);

			Assert.AreEqual(nameof(MockRecord.FieldWithoutDisplayAttribute), fieldDisplayName);
		}

		private class MockRecord
		{
			[Display(Name = "FieldWithDisplayAttributeX")]
			public string FieldWithDisplayAttribute { get; set; }
			public string FieldWithoutDisplayAttribute { get; set; }
		}
	}
}
