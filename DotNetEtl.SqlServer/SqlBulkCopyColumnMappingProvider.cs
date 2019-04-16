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
			var properties = typeof(TRecord).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				var destinationColumnOrdinalAttribute = property.GetCustomAttribute<DestinationFieldOrdinalAttribute>(true);

				if (destinationColumnOrdinalAttribute != null)
				{
					columnMappings.Add(new SqlBulkCopyColumnMapping(property.Name, destinationColumnOrdinalAttribute.Ordinal));
				}
				else
				{
					var destinationColumnNameAttribute = property.GetCustomAttribute<DestinationFieldNameAttribute>(true);

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
