namespace DotNetEtl
{
	public interface IRecordFilter
	{
		bool MeetsCriteria(object record);
	}
}
