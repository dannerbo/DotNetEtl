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
				var couldMapRecord = this.TryMapRecord(record, out object mappedRecord, out failures);

				record = mappedRecord;

				return couldMapRecord;
			}

			failures = null;

			return record != null;
		}

		protected virtual bool TryMapRecord(object record, out object mappedRecord, out IEnumerable<FieldFailure> failures)
		{
			return this.RecordMapper.TryMap(record, out mappedRecord, out failures);
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
