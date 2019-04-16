using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace DotNetEtl.SqlServer.Tests
{
	[TestClass]
	public class DynamicRecordSqlBulkCopyColumnMappingProviderTests
	{
		[TestMethod]
		public void GetColumnMappings_RecordTypeIsNotProvided_ColumnMappingsReturnedAsExpected()
		{
			var fieldWithDestinationFieldName = MockRepository.GenerateMock<IDynamicRecordField>();
			var fieldWithDestinationFieldOrdinal = MockRepository.GenerateMock<IDynamicRecordField>();
			var fieldWithoutDestinationField = MockRepository.GenerateMock<IDynamicRecordField>();

			fieldWithDestinationFieldName.Stub(x => x.DestinationFieldName).Return("FieldWithDestinationFieldName");
			fieldWithDestinationFieldOrdinal.Stub(x => x.DestinationFieldOrdinal).Return(1);

			var dynamicRecordFields = new List<IDynamicRecordField>()
			{
				fieldWithDestinationFieldName,
				fieldWithDestinationFieldOrdinal,
				fieldWithoutDestinationField
			};

			var dynamicRecordColumnProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			dynamicRecordColumnProvider.Stub(x => x.GetFields()).Return(dynamicRecordFields);

			var dynamicRecordSqlBulkCopyColumnMappingProvider = new DynamicRecordSqlBulkCopyColumnMappingProvider(dynamicRecordColumnProvider);

			var columnMappings = dynamicRecordSqlBulkCopyColumnMappingProvider.GetColumnMappings();

			Assert.AreEqual(2, columnMappings.Count());
			Assert.IsNotNull(columnMappings.SingleOrDefault(x => x.DestinationColumn.Equals("FieldWithDestinationFieldName")));
			Assert.IsNotNull(columnMappings.SingleOrDefault(x => x.DestinationOrdinal.Equals(1)));
		}

		[TestMethod]
		public void GetColumnMappings_RecordTypeIsProvided_ColumnMappingsReturnedAsExpected()
		{
			var recordType = "X";
			var fieldWithDestinationFieldName = MockRepository.GenerateMock<IDynamicRecordField>();
			var fieldWithDestinationFieldOrdinal = MockRepository.GenerateMock<IDynamicRecordField>();
			var fieldWithoutDestinationField = MockRepository.GenerateMock<IDynamicRecordField>();

			fieldWithDestinationFieldName.Stub(x => x.DestinationFieldName).Return("FieldWithDestinationFieldName");
			fieldWithDestinationFieldOrdinal.Stub(x => x.DestinationFieldOrdinal).Return(1);

			var dynamicRecordFields = new List<IDynamicRecordField>()
			{
				fieldWithDestinationFieldName,
				fieldWithDestinationFieldOrdinal,
				fieldWithoutDestinationField
			};

			var dynamicRecordColumnProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			dynamicRecordColumnProvider.Stub(x => x.GetFields(Arg<string>.Is.Equal(recordType))).Return(dynamicRecordFields);

			var dynamicRecordSqlBulkCopyColumnMappingProvider = new DynamicRecordSqlBulkCopyColumnMappingProvider(dynamicRecordColumnProvider, recordType);

			var columnMappings = dynamicRecordSqlBulkCopyColumnMappingProvider.GetColumnMappings();

			Assert.AreEqual(2, columnMappings.Count());
			Assert.IsNotNull(columnMappings.SingleOrDefault(x => x.DestinationColumn.Equals("FieldWithDestinationFieldName")));
			Assert.IsNotNull(columnMappings.SingleOrDefault(x => x.DestinationOrdinal.Equals(1)));
		}

		[TestMethod]
		[ExpectedException(typeof(InternalTestFailureException))]
		public void GetColumnMappings_DynamicRecordFieldProviderThrowsException_ExceptionIsPropogated()
		{
			var dynamicRecordColumnProvider = MockRepository.GenerateMock<IDynamicRecordFieldProvider>();

			dynamicRecordColumnProvider.Stub(x => x.GetFields()).Throw(new InternalTestFailureException());

			var dynamicRecordSqlBulkCopyColumnMappingProvider = new DynamicRecordSqlBulkCopyColumnMappingProvider(dynamicRecordColumnProvider);

			dynamicRecordSqlBulkCopyColumnMappingProvider.GetColumnMappings();
		}
	}
}
