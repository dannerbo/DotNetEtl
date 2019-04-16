using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Microsoft.SqlServer.Server;

namespace DotNetEtl.SqlServer
{
	public class SqlDataRecordMapper : ISqlDataRecordMapper
	{
		private static readonly Dictionary<SqlDbType, Type> TypeMapping = new Dictionary<SqlDbType, Type>
		{
			{ SqlDbType.BigInt, typeof(long) },
			{ SqlDbType.Binary, typeof(byte[]) },
			{ SqlDbType.Bit, typeof(bool) },
			{ SqlDbType.Char, typeof(string) },
			{ SqlDbType.Date, typeof(DateTime) },
			{ SqlDbType.DateTime, typeof(DateTime) },
			{ SqlDbType.DateTime2, typeof(DateTime) },
			{ SqlDbType.DateTimeOffset, typeof(DateTimeOffset) },
			{ SqlDbType.Decimal, typeof(decimal) },
			{ SqlDbType.Float, typeof(double) },
			{ SqlDbType.Int, typeof(int) },
			{ SqlDbType.Money, typeof(decimal) },
			{ SqlDbType.NChar, typeof(string) },
			{ SqlDbType.NText, typeof(string) },
			{ SqlDbType.NVarChar, typeof(string) },
			{ SqlDbType.Real, typeof(float) },
			{ SqlDbType.SmallInt, typeof(short) },
			{ SqlDbType.SmallMoney, typeof(decimal) },
			{ SqlDbType.Text, typeof(string) },
			{ SqlDbType.Time, typeof(TimeSpan) },
			{ SqlDbType.Timestamp, typeof(byte[]) },
			{ SqlDbType.TinyInt, typeof(byte) },
			{ SqlDbType.UniqueIdentifier, typeof(Guid) },
			{ SqlDbType.VarBinary, typeof(byte[]) },
			{ SqlDbType.VarChar, typeof(string) }
		};

		private SqlMetaData[] sqlMetaDataList;
		private List<Tuple<PropertyInfo, TableValuedParameterFieldAttribute>> tvpFields;

		public SqlDataRecord Map(object record)
		{
			if (this.sqlMetaDataList == null)
			{
				this.GenerateSqlMetaDataList(record);
			}

			var sqlDataRecord = new SqlDataRecord(this.sqlMetaDataList);
			var tvpFieldValues = new List<object>();

			for (var ordinal = 0; ordinal < this.tvpFields.Count; ordinal++)
			{
				var tvpField = this.tvpFields[ordinal];
				var tvpFieldValue = tvpField.Item1.GetValue(record) ?? DBNull.Value;

				this.TryConvertType(ref tvpFieldValue, tvpField.Item2.SqlDbType);
				
				tvpFieldValues.Add(tvpFieldValue);
			}

			sqlDataRecord.SetValues(tvpFieldValues.ToArray());

			return sqlDataRecord;
		}

		private void GenerateSqlMetaDataList(object record)
		{
			this.tvpFields = new List<Tuple<PropertyInfo, TableValuedParameterFieldAttribute>>();

			var properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				var tvpFieldAttribute = property.GetCustomAttribute<TableValuedParameterFieldAttribute>(true);

				if (tvpFieldAttribute != null)
				{
					this.tvpFields.Add(new Tuple<PropertyInfo, TableValuedParameterFieldAttribute>(property, tvpFieldAttribute));
				}
			}

			this.tvpFields.Sort((x, y) => x.Item2.Ordinal.CompareTo(y.Item2.Ordinal));

			var sqlMetaDataList = new List<SqlMetaData>();

			for (var ordinal = 0; ordinal < this.tvpFields.Count; ordinal++)
			{
				var tvpField = this.tvpFields[ordinal];

				if (tvpField.Item2.Ordinal != ordinal)
				{
					throw new InvalidOperationException($"Failed to find TVP field at ordinal {ordinal}.");
				}

				SqlMetaData sqlMetaData;

				if (tvpField.Item2.MaxLength.HasValue)
				{
					sqlMetaData = new SqlMetaData(tvpField.Item2.Name, tvpField.Item2.SqlDbType, tvpField.Item2.MaxLength.Value);
				}
				else if (tvpField.Item2.Precision.HasValue && tvpField.Item2.Scale.HasValue)
				{
					sqlMetaData = new SqlMetaData(tvpField.Item2.Name, tvpField.Item2.SqlDbType, tvpField.Item2.Precision.Value, tvpField.Item2.Scale.Value);
				}
				else
				{
					sqlMetaData = new SqlMetaData(tvpField.Item2.Name, tvpField.Item2.SqlDbType);
				}

				sqlMetaDataList.Add(sqlMetaData);
			}

			this.sqlMetaDataList = sqlMetaDataList.ToArray();
		}

		private void TryConvertType(ref object value, SqlDbType sqlDbType)
		{
			if (value != DBNull.Value
				&& SqlDataRecordMapper.TypeMapping.TryGetValue(sqlDbType, out var dotNetType)
				&& dotNetType != value.GetType())
			{
				value = Convert.ChangeType(value, dotNetType);
			}
		}
	}
}
