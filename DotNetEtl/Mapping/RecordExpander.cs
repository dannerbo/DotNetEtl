using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetEtl.Mapping
{
	public class RecordExpander : IRecordExpander
	{
		private IRecordMapper[] recordMappers;

		public RecordExpander(params IRecordMapper[] recordMappers)
		{
			this.recordMappers = recordMappers;
		}

		public RecordExpander(params Type[] recordTypes)
		{
			this.recordMappers = this.CreateRecordMappers(recordTypes);
		}

		public IEnumerable<object> Expand(object record)
		{
			if (record == null)
			{
				throw new ArgumentNullException(nameof(record));
			}

			var expandedRecords = new List<object>();

			foreach (var recordMapper in this.recordMappers)
			{
				if (recordMapper.TryMap(record, out var mappedRecord, out var failures))
				{
					expandedRecords.Add(mappedRecord);
				}
				else if (failures?.Count() > 0)
				{
					throw new InvalidOperationException("Record failed to expand.");
				}
			}

			return expandedRecords;
		}

		protected virtual IRecordMapper[] CreateRecordMappers(Type[] recordTypes)
		{
			var recordMappers = new List<IRecordMapper>();

			foreach (var recordType in recordTypes)
			{
				var recordFactory = new RecordFactory((src, rt) => Activator.CreateInstance(recordType));
				var recordMapper = new ObjectRecordMapper(recordFactory);

				recordMappers.Add(recordMapper);
			}

			return recordMappers.ToArray();
		}
	}
}
