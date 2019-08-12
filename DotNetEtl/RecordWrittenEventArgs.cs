namespace DotNetEtl
{
	public class RecordWrittenEventArgs : RecordEventArgs
	{
		public RecordWrittenEventArgs(int recordIndex, object record, object formattedRecord)
			: base(recordIndex, record)
		{
			this.FormattedRecord = formattedRecord;
		}

		public object FormattedRecord { get; private set; }
	}
}
