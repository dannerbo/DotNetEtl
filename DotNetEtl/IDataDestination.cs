namespace DotNetEtl
{
	public interface IDataDestination
	{
		IRecordFilter RecordFilter { get; }

		IDataWriter CreateDataWriter(IDataSource dataSource);
	}
}
