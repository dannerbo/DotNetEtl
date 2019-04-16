namespace DotNetEtl
{
	public class RecordWrittenEventArgs : RecordEventArgs
	{
		public RecordWrittenEventArgs(object record, object formattedRecord)
			: base(record)
		{
			this.FormattedRecord = formattedRecord;
		}

		public object FormattedRecord { get; private set; }
	}
}
