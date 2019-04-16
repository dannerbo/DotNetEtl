namespace DotNetEtl
{
	public class RecordFormattedEventArgs : RecordEventArgs
	{
		public RecordFormattedEventArgs(object record, object formattedRecord)
			: base(record)
		{
			this.FormattedRecord = formattedRecord;
		}

		public object FormattedRecord { get; private set; }
	}
}
