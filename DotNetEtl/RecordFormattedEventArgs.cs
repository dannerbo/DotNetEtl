namespace DotNetEtl
{
	public class RecordFormattedEventArgs : RecordEventArgs
	{
		public RecordFormattedEventArgs(int recordIndex, object record, object formattedRecord)
			: base(recordIndex, record)
		{
			this.FormattedRecord = formattedRecord;
		}

		public object FormattedRecord { get; private set; }
	}
}
