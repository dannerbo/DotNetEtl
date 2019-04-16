using System;

namespace DotNetEtl
{
	public class RecordFactory : IRecordFactory
	{
		public RecordFactory(Func<object, object, object> createRecord, IRecordTypeProvider recordTypeProvider = null)
		{
			this.CreateRecord = createRecord;
			this.RecordTypeProvider = recordTypeProvider;
		}

		protected Func<object, object, object> CreateRecord { get; private set; }
		protected IRecordTypeProvider RecordTypeProvider { get; private set; }

		public object Create(object source)
		{
			return this.CreateRecord(source, this.RecordTypeProvider?.GetRecordType(source));
		}
	}

	public class RecordFactory<TRecord> : RecordFactory, IRecordFactory<TRecord>
		where TRecord : class, new()
	{
		public RecordFactory()
			: base((source, recordType) => new TRecord())
		{
		}
	}
}
