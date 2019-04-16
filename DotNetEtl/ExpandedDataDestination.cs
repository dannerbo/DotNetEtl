using System.Collections.Generic;

namespace DotNetEtl
{
	public class ExpandedDataDestination : IDataDestination
	{
		public ExpandedDataDestination(IRecordExpander recordExpander, IRecordFilter recordFilter, params IDataDestination[] dataDestinations)
		{
			this.RecordExpander = recordExpander;
			this.DataDestinations = dataDestinations;
			this.RecordFilter = recordFilter;
		}

		public ExpandedDataDestination(IRecordExpander recordExpander, params IDataDestination[] dataDestinations)
			: this(recordExpander, null, dataDestinations)
		{
		}

		public int MaxDegreeOfParallelismForExpandedRecords { get; set; } = -1;
		public int MaxDegreeOfParallelismForDataWriters { get; set; } = -1;
		public IRecordFilter RecordFilter { get; private set; }
		protected IRecordExpander RecordExpander { get; private set; }
		protected IEnumerable<IDataDestination> DataDestinations { get; private set; }

		public IDataWriter CreateDataWriter(IDataSource dataSource)
		{
			var dataWriters = new Dictionary<IDataDestination, IDataWriter>();

			foreach (var dataDestination in this.DataDestinations)
			{
				var dataWriter = dataDestination.CreateDataWriter(dataSource);

				dataWriters.Add(dataDestination, dataWriter);
			}

			return new ExpandedDataWriter(dataWriters, this.RecordExpander)
			{
				MaxDegreeOfParallelismForExpandedRecords = this.MaxDegreeOfParallelismForExpandedRecords,
				MaxDegreeOfParallelismForDataWriters = this.MaxDegreeOfParallelismForDataWriters
			};
		}
	}
}
