using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Transactions;

namespace DotNetEtl.SqlServer
{
	public class SqlBulkCopyWriter : DataWriter
	{
		private SqlConnection sqlConnection;
		private SqlTransaction sqlTransaction;
		private Task bulkCopyTask;
		private ObjectDataReader dataReader;
		private CancellationTokenSource cancellationTokenSource;

		public SqlBulkCopyWriter(
			ISqlBulkCopyColumnMappingProvider columnMappingProvider,
			string connectionString,
			string destinationTableName)
		{
			this.ColumnMappingProvider = columnMappingProvider;
			this.ConnectionString = connectionString;
			this.DestinationTableName = destinationTableName;
		}

		public SqlBulkCopyOptions CopyOptions { get; set; } = SqlBulkCopyOptions.Default;
		public int BatchSize { get; set; } = 1000;
		public int Timeout { get; set; }
		public bool IsStreamingEnabled { get; set; } = true;
		protected ISqlBulkCopyColumnMappingProvider ColumnMappingProvider { get; private set; }
		protected string ConnectionString { get; private set; }
		protected string DestinationTableName { get; private set; }
		protected SqlBulkCopy BulkCopy { get; private set; }

		public override void Close()
		{
			try
			{
				if (this.bulkCopyTask != null
					&& !(this.bulkCopyTask.IsCanceled || this.bulkCopyTask.IsCompleted || this.bulkCopyTask.IsFaulted))
				{
					this.Rollback();
				}
			}
			finally
			{
				if (this.dataReader != null)
				{
					this.dataReader.Dispose();
					this.dataReader = null;
				}

				if (this.bulkCopyTask != null)
				{
					this.bulkCopyTask.Dispose();
					this.bulkCopyTask = null;
				}

				if (this.BulkCopy != null)
				{
					((IDisposable)this.BulkCopy).Dispose();
					this.BulkCopy = null;
				}

				if (this.sqlTransaction != null)
				{
					this.sqlTransaction.Dispose();
					this.sqlTransaction = null;
				}

				if (this.sqlConnection != null)
				{
					this.sqlConnection.Dispose();
					this.sqlConnection = null;
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
			if (this.dataReader == null)
			{
				this.Initialize(record.GetType());
			}

			this.dataReader.PostRecord(record);
		}

		public override void Commit()
		{
			if (this.dataReader != null)
			{
				this.dataReader.Complete();
				this.bulkCopyTask.Wait();
				this.sqlTransaction?.Commit();
			}
		}

		public override void Rollback()
		{
			try
			{
				this.dataReader?.Complete();
				ActionHelper.PerformErrorableAction(() => this.bulkCopyTask?.Wait());
			}
			finally
			{
				this.sqlTransaction?.Rollback();
			}
		}

		protected virtual void Initialize(Type recordType)
		{
			this.cancellationTokenSource = new CancellationTokenSource();
			this.sqlConnection = this.CreateSqlConnection();
			this.sqlTransaction = this.CreateSqlTransaction(this.sqlConnection);
			this.BulkCopy = this.CreateBulkCopy(this.sqlConnection, this.sqlTransaction);
			this.dataReader = new ObjectDataReader(recordType, this.cancellationTokenSource.Token);
			this.bulkCopyTask = Task.Run(() =>
			{
				try
				{
					this.BulkCopy.WriteToServer(this.dataReader);
				}
				catch
				{
					this.cancellationTokenSource.Cancel();

					throw;
				}
			});
		}

		protected virtual SqlConnection CreateSqlConnection()
		{
			var connection = new SqlConnection(this.ConnectionString);

			connection.Open();

			return connection;
		}

		protected virtual SqlTransaction CreateSqlTransaction(SqlConnection connection)
		{
			if ((this.CopyOptions & SqlBulkCopyOptions.UseInternalTransaction) == SqlBulkCopyOptions.UseInternalTransaction)
			{
				return null;
			}

			return Transaction.Current == null
				? connection.BeginTransaction()
				: null;
		}

		protected virtual SqlBulkCopy CreateBulkCopy(SqlConnection connection, SqlTransaction transaction)
		{
			var bulkCopy = new SqlBulkCopy(connection, this.CopyOptions, transaction);
			bulkCopy.BatchSize = this.BatchSize;
			bulkCopy.EnableStreaming = this.IsStreamingEnabled;
			bulkCopy.BulkCopyTimeout = this.Timeout;
			bulkCopy.DestinationTableName = this.DestinationTableName;

			this.AddColumnMappings(bulkCopy);

			return bulkCopy;
		}

		protected virtual void AddColumnMappings(SqlBulkCopy bulkCopy)
		{
			if (this.ColumnMappingProvider != null)
			{
				foreach (var columnMapping in this.ColumnMappingProvider.GetColumnMappings())
				{
					bulkCopy.ColumnMappings.Add(columnMapping);
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}

		protected sealed class ObjectDataReader : System.Data.IDataReader
		{
			private delegate object GetMethod(object target);

			private GetMethod[] accessors;
			private Dictionary<string, int> ordinalLookup;
			private BufferBlock<object> recordQueue = new BufferBlock<object>();
			private object currentRecord;
			private CancellationToken cancellationToken;

			public ObjectDataReader(Type recordType, CancellationToken cancellationToken)
			{
				this.cancellationToken = cancellationToken;

				this.Initialize(recordType);
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

			private void Initialize(Type recordType)
			{
				var propertyAccessors = recordType.GetCachedProperties()
					.Where(p => p.CanRead)
					.Select((p, i) => new
					{
						Index = i,
						Property = p,
						Accessor = this.CreatePropertyAccessor(p)
					})
					.ToArray();

				this.accessors = propertyAccessors.Select(p => p.Accessor).ToArray();
				this.ordinalLookup = propertyAccessors.ToDictionary(
					p => p.Property.Name,
					p => p.Index,
					StringComparer.OrdinalIgnoreCase);
			}

			private GetMethod CreatePropertyAccessor(PropertyInfo property)
			{
				var getMethod = property.GetGetMethod();
				var arguments = new Type[] { typeof(object) };
				var getter = new DynamicMethod("__Get" + property.Name, typeof(object), arguments, property.DeclaringType);
				var generator = getter.GetILGenerator();

				generator.DeclareLocal(typeof(object));
				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Castclass, property.DeclaringType);
				generator.EmitCall(OpCodes.Callvirt, getMethod, null);

				if (!property.PropertyType.IsClass)
				{
					generator.Emit(OpCodes.Box, property.PropertyType);
				}

				generator.Emit(OpCodes.Ret);

				return (GetMethod)getter.CreateDelegate(typeof(GetMethod));
			}

			#region IDataReader Members

			public void Close()
			{
				this.Dispose();
			}

			public int Depth
			{
				get { return 1; }
			}

			public DataTable GetSchemaTable()
			{
				return null;
			}

			public bool IsClosed
			{
				get { return this.recordQueue == null; }
			}

			public bool NextResult()
			{
				return false;
			}

			public bool Read()
			{
				try
				{
					this.currentRecord = this.recordQueue.Receive();

					return true;
				}
				catch (InvalidOperationException invalidOperationException)
				{
					if (invalidOperationException.HResult == -2146233079)
					{
						return false;
					}

					throw;
				}
			}

			public int RecordsAffected
			{
				get { return -1; }
			}

			#endregion

			#region IDataRecord Members

			public int FieldCount
			{
				get { return this.accessors.Length; }
			}

			public bool GetBoolean(int i)
			{
				throw new NotImplementedException();
			}

			public byte GetByte(int i)
			{
				throw new NotImplementedException();
			}

			public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
			{
				throw new NotImplementedException();
			}

			public char GetChar(int i)
			{
				throw new NotImplementedException();
			}

			public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
			{
				throw new NotImplementedException();
			}

			public System.Data.IDataReader GetData(int i)
			{
				throw new NotImplementedException();
			}

			public string GetDataTypeName(int i)
			{
				throw new NotImplementedException();
			}

			public DateTime GetDateTime(int i)
			{
				throw new NotImplementedException();
			}

			public decimal GetDecimal(int i)
			{
				throw new NotImplementedException();
			}

			public double GetDouble(int i)
			{
				throw new NotImplementedException();
			}

			public Type GetFieldType(int i)
			{
				throw new NotImplementedException();
			}

			public float GetFloat(int i)
			{
				throw new NotImplementedException();
			}

			public Guid GetGuid(int i)
			{
				throw new NotImplementedException();
			}

			public short GetInt16(int i)
			{
				throw new NotImplementedException();
			}

			public int GetInt32(int i)
			{
				throw new NotImplementedException();
			}

			public long GetInt64(int i)
			{
				throw new NotImplementedException();
			}

			public string GetName(int i)
			{
				throw new NotImplementedException();
			}

			public int GetOrdinal(string name)
			{
				if (!this.ordinalLookup.TryGetValue(name, out int ordinal))
				{
					throw new InvalidOperationException($"Unknown field name '{name}'.");
				}

				return ordinal;
			}

			public string GetString(int i)
			{
				throw new NotImplementedException();
			}

			public object GetValue(int i)
			{
				return this.accessors[i](this.currentRecord);
			}

			public int GetValues(object[] values)
			{
				throw new NotImplementedException();
			}

			public bool IsDBNull(int i)
			{
				object fieldValue = this.GetValue(i);

				return fieldValue == null || Convert.IsDBNull(fieldValue);
			}

			public object this[string name]
			{
				get { throw new NotImplementedException(); }
			}

			public object this[int i]
			{
				get { throw new NotImplementedException(); }
			}

			#endregion

			#region IDisposable Members

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			private void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.recordQueue = null;
				}
			}

			#endregion
		}
	}

	public class SqlBulkCopyWriter<TRecord> : SqlBulkCopyWriter
	{
		public SqlBulkCopyWriter(string connectionString, string destinationTableName)
			: base(new SqlBulkCopyColumnMappingProvider<TRecord>(), connectionString, destinationTableName)
		{
		}
	}
}
