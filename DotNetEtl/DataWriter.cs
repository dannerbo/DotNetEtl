using System;

namespace DotNetEtl
{
	public abstract class DataWriter : IDataWriter
	{
		protected DataWriter(IRecordFormatter recordFormatter = null)
		{
			this.RecordFormatter = recordFormatter;
		}

		public event EventHandler<RecordFormattedEventArgs> RecordFormatted;

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
			record = this.FormatRecord(record);
			
			this.WriteRecordInternal(record);
		}

		protected virtual object FormatRecord(object record)
		{
			if (this.RecordFormatter != null)
			{
				var formattedRecord = this.RecordFormatter.Format(record);

				this.OnRecordFormatted(record, formattedRecord);

				return formattedRecord;
			}

			return record;
		}

		protected virtual void OnRecordFormatted(object record, object formattedRecord)
		{
			this.RecordFormatted?.Invoke(this, new RecordFormattedEventArgs(record, formattedRecord));
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
