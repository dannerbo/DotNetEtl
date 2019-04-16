using System;
using System.Collections.Generic;

namespace DotNetEtl
{
	public abstract class DataReader : IDataReader
	{
		protected DataReader(IRecordMapper recordMapper = null)
		{
			this.RecordMapper = recordMapper;
		}

		public event EventHandler<RecordMappedEventArgs> RecordMapped;

		protected IRecordMapper RecordMapper { get; private set; }

		protected abstract object ReadRecordInternal();

		public virtual void Open()
		{
		}

		public virtual void Close()
		{
		}
		
		public virtual bool TryReadRecord(out object record, out IEnumerable<FieldFailure> failures)
		{
			record = this.ReadRecordInternal();

			if (record != null && this.RecordMapper != null)
			{
				var couldMapRecord = this.RecordMapper.TryMap(record, out object mappedRecord, out failures);

				this.OnRecordMapped(record, couldMapRecord, failures, mappedRecord);

				record = mappedRecord;

				return couldMapRecord;
			}

			failures = null;

			return record != null;
		}

		protected virtual void OnRecordMapped(object record, bool wasSuccessful, IEnumerable<FieldFailure> failures, object mappedRecord)
		{
			this.RecordMapped?.Invoke(this, new RecordMappedEventArgs(record, wasSuccessful, failures, mappedRecord));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
		}
	}
}
