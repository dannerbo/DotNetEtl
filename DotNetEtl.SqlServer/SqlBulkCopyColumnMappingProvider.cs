using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace DotNetEtl.SqlServer
{
	public class SqlBulkCopyColumnMappingProvider<TRecord> : ISqlBulkCopyColumnMappingProvider
	{
		public IEnumerable<SqlBulkCopyColumnMapping> GetColumnMappings()
		{
			var columnMappings = new List<SqlBulkCopyColumnMapping>();
			var properties = typeof(TRecord).GetCachedProperties();

			foreach (var property in properties)
			{
				var destinationColumnOrdinalAttribute = property.GetCachedCustomAttribute<DestinationFieldOrdinalAttribute>();

				if (destinationColumnOrdinalAttribute != null)
				{
					columnMappings.Add(new SqlBulkCopyColumnMapping(property.Name, destinationColumnOrdinalAttribute.Ordinal));
				}
				else
				{
					var destinationColumnNameAttribute = property.GetCachedCustomAttribute<DestinationFieldNameAttribute>();

					if (destinationColumnNameAttribute != null)
					{
						columnMappings.Add(new SqlBulkCopyColumnMapping(property.Name, destinationColumnNameAttribute.FieldName));
					}
				}
			}

			return columnMappings;
		}
	}
}
