using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetEtl
{
	public class ExpandedDataWriter : IDataWriter
	{
		public ExpandedDataWriter(IDictionary<IDataDestination, IDataWriter> dataWriters, IRecordExpander recordExpander)
		{
			this.DataWriters = dataWriters;
			this.RecordExpander = recordExpander;
		}

		public int MaxDegreeOfParallelismForExpandedRecords { get; set; } = -1;
		public int MaxDegreeOfParallelismForDataWriters { get; set; } = -1;
		protected IDictionary<IDataDestination, IDataWriter> DataWriters { get; private set; }
		protected IRecordExpander RecordExpander { get; private set; }

		public void Open()
		{
			this.ForEachDataWriter(dataWriter => dataWriter.Open());
		}

		public void Close()
		{
			this.ForEachDataWriter(dataWriter => dataWriter.Close());
		}

		public void WriteRecord(object record)
		{
			var expandedRecords = this.RecordExpander.Expand(record);
			var expandedRecordsParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelismForExpandedRecords };

			Parallel.ForEach(expandedRecords, expandedRecordsParallelOptions, expandedRecord =>
			{
				var dataWriters = this.GetFilteredDataWriters(expandedRecord);
				var dataWriteresParallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelismForDataWriters };

				Parallel.ForEach(dataWriters, dataWriteresParallelOptions, dataWriter => dataWriter.WriteRecord(expandedRecord));
			});
		}

		public void Commit()
		{
			this.ForEachDataWriter(dataWriter => dataWriter.Commit());
		}

		public void Rollback()
		{
			this.ForEachDataWriter(dataWriter => dataWriter.Rollback());
		}

		public void Dispose()
		{
			this.ForEachDataWriter(dataWriter => dataWriter.Dispose());
		}

		protected virtual void ForEachDataWriter(Action<IDataWriter> action)
		{
			var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = this.MaxDegreeOfParallelismForDataWriters };

			Parallel.ForEach(this.DataWriters.Values, parallelOptions, dataWriter => action(dataWriter));
		}

		protected IEnumerable<IDataWriter> GetFilteredDataWriters(object record)
		{
			var dataWriters = new List<IDataWriter>();
			var filteredDataDestinations = this.DataWriters.Keys.Where(x => x.RecordFilter == null || x.RecordFilter.MeetsCriteria(record));

			foreach (var dataDestination in filteredDataDestinations)
			{
				dataWriters.Add(this.DataWriters[dataDestination]);
			}

			return dataWriters;
		}
	}
}
