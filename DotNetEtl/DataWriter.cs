using System;

namespace DotNetEtl
{
	public abstract class DataWriter : IDataWriter
	{
		protected DataWriter(IRecordFormatter recordFormatter = null)
		{
			this.RecordFormatter = recordFormatter;
		}

		protected IRecordFormatter RecordFormatter { get; private set; }

		protected abstract void WriteRecordInternal(object record);
		public abstract void Commit();
		public abstract void Rollback();

		public virtual void Open()
		{
		}

		public virtual void Close()
		{
		}

		public virtual void WriteRecord(object record)
		{
			if (this.RecordFormatter != null)
			{
				record = this.FormatRecord(record);
			}
			
			this.WriteRecordInternal(record);
		}

		protected virtual object FormatRecord(object record)
		{
			return this.RecordFormatter.Format(record);
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
