using System;
using System.Data;

namespace DotNetEtl.SqlServer
{
	public class SqlParameterAttribute : Attribute
	{
		public SqlParameterAttribute(string name)
		{
			this.Name = name;
		}
		public SqlParameterAttribute(string name, SqlDbType sqlDbType)
			: this(name)
		{
			this.SqlDbType = sqlDbType;
		}

		public SqlParameterAttribute(string name, SqlDbType sqlDbType, int size)
			: this(name, sqlDbType)
		{
			this.Size = size;
		}

		public SqlParameterAttribute(string name, SqlDbType sqlDbType, byte precision, byte scale)
			: this(name, sqlDbType)
		{
			this.Precision = precision;
			this.Scale = scale;
		}

		public string Name { get; private set; }
		public SqlDbType? SqlDbType { get; private set; }
		public int? Size { get; private set; }
		public byte? Precision { get; private set; }
		public byte? Scale { get; private set; }
	}
}
