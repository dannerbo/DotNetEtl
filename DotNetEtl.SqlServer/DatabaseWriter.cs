using System.Data.SqlClient;

namespace DotNetEtl.SqlServer
{
	public class DatabaseWriter : DataWriter
	{
		private string connectionString;
		private bool shouldUseTransaction;
		private SqlConnection connection;
		private SqlTransaction transaction;
		
		public DatabaseWriter(IDatabaseWriterCommandFactory commandFactory, string connectionString, bool shouldUseTransaction = true)
		{
			this.CommandFactory = commandFactory;
			this.connectionString = connectionString;
			this.shouldUseTransaction = shouldUseTransaction;
		}

		protected IDatabaseWriterCommandFactory CommandFactory { get; private set; }

		public override void Open()
		{
			this.connection = new SqlConnection(this.connectionString);

			this.connection.Open();

			if (this.shouldUseTransaction)
			{
				this.transaction = connection.BeginTransaction();
			}
		}

		public override void Close()
		{
			if (this.transaction != null)
			{
				this.transaction.Dispose();
				this.transaction = null;
			}

			if (this.connection != null)
			{
				this.connection.Dispose();
				this.connection = null;
			}
		}

		protected override void WriteRecordInternal(object record)
		{
			using (var command = this.CommandFactory.Create(record))
			{
				command.Connection = this.connection;

				if (this.transaction != null)
				{
					command.Transaction = this.transaction;
				}

				command.ExecuteNonQuery();
			}
		}

		public override void Commit()
		{
			this.transaction?.Commit();
		}

		public override void Rollback()
		{
			this.transaction?.Rollback();
		}
	}
}
