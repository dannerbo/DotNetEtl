using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Transactions;
using Microsoft.SqlServer.Server;

namespace DotNetEtl.SqlServer
{
	public class DatabaseBatchWriter : DataWriter
	{
		private SqlCommand command;
		private ISqlDataRecordMapper sqlDataRecordMapper;
		private SqlParameter tvpParameter;
		private SqlTransaction transaction;
		private string tvpParameterName;
		private bool disposeConnection;
		private SqlDataRecordEnumerator tvpRecordEnumerator;
		private Task executeCommandTask;
		private CancellationTokenSource cancellationTokenSource;

		public DatabaseBatchWriter(SqlCommand command, ISqlDataRecordMapper sqlDataRecordMapper, string tvpParameterName, bool disposeConnection = true)
		{
			this.command = command;
			this.sqlDataRecordMapper = sqlDataRecordMapper;
			this.tvpParameterName = tvpParameterName;
			this.disposeConnection = disposeConnection;
		}

		public DatabaseBatchWriter(SqlCommand command, string tvpParameterName, bool disposeConnection = true)
			: this(command, new SqlDataRecordMapper(), tvpParameterName, disposeConnection)
		{
		}

		public DatabaseBatchWriter(SqlCommand command, bool disposeConnection = true)
			: this(command, null, disposeConnection)
		{
		}

		public override void Open()
		{
			this.tvpParameter = this.GetTvpParameter();

			this.ThrowIfTvpParameterNotFound();
			this.ThrowIfTvpParameterTypeInvalid();
			this.ThrowIfTvpParameterHasValue();
			this.ThrowIfConnectionNotProvided();

			if (this.command.Connection.State != ConnectionState.Open)
			{
				this.command.Connection.Open();
			}

			if (this.command.Transaction == null && Transaction.Current == null)
			{
				this.command.Transaction = this.transaction = this.command.Connection.BeginTransaction();
			}

			this.cancellationTokenSource = new CancellationTokenSource();
			this.tvpRecordEnumerator = new SqlDataRecordEnumerator(this.sqlDataRecordMapper, this.cancellationTokenSource.Token);
			this.tvpParameter.Value = this.tvpRecordEnumerator;
			this.executeCommandTask = Task.Run(() =>
				{
					try
					{
						this.command.ExecuteNonQuery();
					}
					catch (ArgumentException argumentException)
					{
						if (argumentException.HResult == -2147024809)
						{
							return;
						}

						this.cancellationTokenSource.Cancel();

						throw;
					}
					catch
					{
						this.cancellationTokenSource.Cancel();

						throw;
					}
				});
		}

		public override void Close()
		{
			try
			{
				if (this.executeCommandTask != null
					&& !(this.executeCommandTask.IsCanceled || this.executeCommandTask.IsCompleted || this.executeCommandTask.IsFaulted))
				{
					this.Rollback();
				}
			}
			finally
			{
				var connection = this.command?.Connection;

				if (this.transaction != null)
				{
					this.transaction.Dispose();
					this.transaction = null;
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

				if (this.cancellationTokenSource != null)
				{
					this.cancellationTokenSource.Dispose();
					this.cancellationTokenSource = null;
				}
			}
		}

		protected override void WriteRecordInternal(object record)
		{
			this.tvpRecordEnumerator.PostRecord(record);
		}

		public override void Commit()
		{
			this.tvpRecordEnumerator.Complete();
			this.executeCommandTask.Wait();
			this.transaction?.Commit();
		}

		public override void Rollback()
		{
			try
			{
				this.tvpRecordEnumerator.Complete();
				ActionHelper.PerformErrorableAction(() => this.executeCommandTask.Wait());
			}
			finally
			{
				this.transaction?.Rollback();
			}
		}

		private SqlParameter GetTvpParameter()
		{
			if (this.tvpParameterName != null)
			{
				return this.command.Parameters[this.tvpParameterName];
			}
			else
			{
				SqlParameter tvpParameter = null;

				foreach (SqlParameter parameter in this.command.Parameters)
				{
					if (parameter.SqlDbType == SqlDbType.Structured)
					{
						if (tvpParameter != null)
						{
							throw new InvalidOperationException("More than one parameter of SqlDbType 'Structured' was found.");
						}

						tvpParameter = parameter;
					}
				}

				return tvpParameter;
			}
		}
		
		private void ThrowIfTvpParameterNotFound()
		{
			if (this.tvpParameter == null)
			{
				throw new InvalidOperationException("TVP parameter was not found.");
			}
		}

		private void ThrowIfTvpParameterTypeInvalid()
		{
			if (this.tvpParameter.SqlDbType != SqlDbType.Structured)
			{
				throw new InvalidOperationException("TVP parameter SqlDbType must be Structured.");
			}
		}

		private void ThrowIfConnectionNotProvided()
		{
			if (this.command.Connection == null)
			{
				throw new InvalidOperationException("Connection must be provided on the command.");
			}
		}

		private void ThrowIfTvpParameterHasValue()
		{
			if (this.tvpParameter.Value != null)
			{
				throw new InvalidOperationException("TVP parameter should be NULL.");
			}
		}

		protected sealed class SqlDataRecordEnumerator : IEnumerable<SqlDataRecord>
		{
			private BufferBlock<object> recordQueue = new BufferBlock<object>();
			private ISqlDataRecordMapper sqlDataRecordMapper;
			private CancellationToken cancellationToken;

			public SqlDataRecordEnumerator(ISqlDataRecordMapper sqlDataRecordMapper, CancellationToken cancellationToken)
			{
				this.sqlDataRecordMapper = sqlDataRecordMapper;
				this.cancellationToken = cancellationToken;
			}

			public void PostRecord(object record)
			{
				this.recordQueue.Post(record);
			}

			public void Complete()
			{
				this.recordQueue.Complete();
				ActionHelper.PerformCancelableAction(() => this.recordQueue.Completion.Wait(this.cancellationToken));
			}

			public IEnumerator<SqlDataRecord> GetEnumerator()
			{
				object record;

				while (true)
				{
					try
					{
						record = this.recordQueue.Receive();
					}
					catch (InvalidOperationException invalidOperationException)
					{
						if (invalidOperationException.HResult == -2146233079)
						{
							yield break;
						}

						throw;
					}

					yield return this.sqlDataRecordMapper.Map(record);
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}
	}
}
