namespace DotNetEtl
{
	public class RecordFilter<TRecord> : IRecordFilter
	{
		public virtual bool MeetsCriteria(object record)
		{
			return record is TRecord;
		}
	}
}
