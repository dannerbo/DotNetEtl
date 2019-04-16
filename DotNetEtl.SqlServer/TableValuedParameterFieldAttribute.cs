using System;
using System.Data;

namespace DotNetEtl.SqlServer
{
	public class TableValuedParameterFieldAttribute : Attribute
	{
		public TableValuedParameterFieldAttribute(int ordinal, string name, SqlDbType sqlDbType)
		{
			this.Ordinal = ordinal;
			this.Name = name;
			this.SqlDbType = sqlDbType;
		}

		public TableValuedParameterFieldAttribute(int ordinal, string name, SqlDbType sqlDbType, long maxLength)
			: this(ordinal, name, sqlDbType)
		{
			this.MaxLength = maxLength;
		}

		public TableValuedParameterFieldAttribute(int ordinal, string name, SqlDbType sqlDbType, byte precision, byte scale)
			: this(ordinal, name, sqlDbType)
		{
			this.Precision = precision;
			this.Scale = scale;
		}

		public int Ordinal { get; private set; }
		public string Name { get; private set; }
		public SqlDbType SqlDbType { get; private set; }
		public long? MaxLength { get; private set; }
		public byte? Precision { get; private set; }
		public byte? Scale { get; private set; }
	}
}
