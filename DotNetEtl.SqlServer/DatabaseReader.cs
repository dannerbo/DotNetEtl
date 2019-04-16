using System.Data;
using System.Data.SqlClient;
using DotNetEtl.Mapping;

namespace DotNetEtl.SqlServer
{
	public class DatabaseReader : DataReader
	{
		private SqlCommand command;
		private bool disposeConnection;
		private SqlDataReader dataReader;

		public DatabaseReader(IRecordMapper recordMapper, SqlCommand command, bool disposeConnection = true)
			: base(recordMapper)
		{
			this.command = command;
			this.disposeConnection = disposeConnection;
		}

		public override void Open()
		{
			if (this.command.Connection.State != ConnectionState.Open)
			{
				this.command.Connection.Open();
			}

			this.dataReader = this.CreateSqlDataReader(this.command);
		}

		public override void Close()
		{
			var connection = this.command?.Connection;

			if (this.dataReader != null)
			{
				this.dataReader.Dispose();
				this.dataReader = null;
			}

			if (this.command != null)
			{
				this.command.Dispose();
				this.command = null;
			}

			if (this.disposeConnection)
			{
				connection?.Dispose();
			}
		}

		protected override object ReadRecordInternal()
		{
			if (!this.dataReader.Read())
			{
				return null;
			}

			return this.dataReader;
		}

		protected virtual SqlDataReader CreateSqlDataReader(SqlCommand command)
		{
			return command.ExecuteReader();
		}
	}

	public class DatabaseReader<TRecord> : DatabaseReader
		where TRecord : class, new()
	{
		public DatabaseReader(SqlCommand sqlCommand, bool disposeConnection = true)
			: base(new DataRecordRecordMapper<TRecord>(), sqlCommand, disposeConnection)
		{
		}
	}
}
