using System.Collections.Generic;
using System.Data.SqlClient;

namespace DotNetEtl.SqlServer
{
	public class DynamicRecordSqlBulkCopyColumnMappingProvider : ISqlBulkCopyColumnMappingProvider
	{
		public DynamicRecordSqlBulkCopyColumnMappingProvider(IDynamicRecordFieldProvider dynamicRecordColumnProvider)
		{
			this.DynamicRecordColumnProvider = dynamicRecordColumnProvider;
		}

		public DynamicRecordSqlBulkCopyColumnMappingProvider(IDynamicRecordFieldProvider dynamicRecordColumnProvider, string recordType)
			: this(dynamicRecordColumnProvider)
		{
			this.RecordType = recordType;
		}

		protected IDynamicRecordFieldProvider DynamicRecordColumnProvider { get; private set; }
		protected string RecordType { get; private set; }

		public IEnumerable<SqlBulkCopyColumnMapping> GetColumnMappings()
		{
			var columnMappings = new List<SqlBulkCopyColumnMapping>();

			var columns = this.RecordType != null
				? this.DynamicRecordColumnProvider.GetFields(this.RecordType)
				: this.DynamicRecordColumnProvider.GetFields();

			foreach (var column in columns)
			{
				SqlBulkCopyColumnMapping columnMapping = null;

				if (column.DestinationFieldOrdinal != null)
				{
					columnMapping = new SqlBulkCopyColumnMapping(column.Name, column.DestinationFieldOrdinal.Value);
				}
				else if (column.DestinationFieldName != null)
				{
					columnMapping = new SqlBulkCopyColumnMapping(column.Name, column.DestinationFieldName);
				}

				if (columnMapping != null)
				{
					columnMappings.Add(columnMapping);
				}
			}

			return columnMappings;
		}
	}
}
