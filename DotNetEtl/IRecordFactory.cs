namespace DotNetEtl
{
	public interface IRecordFactory
	{
		object Create(object source);
	}

	public interface IRecordFactory<TRecord> : IRecordFactory
		where TRecord : class, new()
	{
	}
}
