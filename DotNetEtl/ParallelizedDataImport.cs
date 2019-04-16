using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DotNetEtl
{
	public class ParallelizedDataImport : DataImport
	{
		private TransformBlock<Tuple<object, int>, Tuple<object, object, IEnumerable<IDataWriter>>> processRecordQueue;
		private ActionBlock<Tuple<object, object, IEnumerable<IDataWriter>>> writeRecordQueue;

		public ParallelizedDataImport(
			IDataSource dataSource,
			IDataDestination dataDestination,
			IRecordValidator recordValidator = null,
			IRecordMapper recordMapper = null,
			IRecordFormatter recordFormatter = null)
			: base(dataSource, dataDestination, recordValidator, recordMapper, recordFormatter)
		{
		}

		public ParallelizedDataImport(
			IDataSource dataSource,
			IEnumerable<IDataDestination> dataDestinations,
			IRecordValidator recordValidator = null,
			IRecordMapper recordMapper = null,
			IRecordFormatter recordFormatter = null)
			: base(dataSource, dataDestinations, recordValidator, recordMapper, recordFormatter)
		{
		}

		public int MaxDegreeOfParallelism { get; set; } = DataflowBlockOptions.Unbounded;

		protected override void PreRun()
		{
			base.PreRun();

			this.CreateDataFlowBlocks();
		}

		protected override void PreCommitOrRollback()
		{
			this.WaitForCompletion();

			base.PreCommitOrRollback();
		}

		protected virtual void CreateDataFlowBlocks()
		{
			this.processRecordQueue = new TransformBlock<Tuple<object, int>, Tuple<object, object, IEnumerable<IDataWriter>>>(
				tuple =>
				{
					try
					{
						base.ProcessRecord(tuple.Item1, tuple.Item2, out var filteredDataWriters, out var mappedRecord, out var formattedRecord);

						return new Tuple<object, object, IEnumerable<IDataWriter>>(mappedRecord, formattedRecord, filteredDataWriters);
					}
					catch (RecordFailedException)
					{
						return null;
					}
				},
				new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = this.MaxDegreeOfParallelism, CancellationToken = this.CancellationToken });

			this.writeRecordQueue = new ActionBlock<Tuple<object, object, IEnumerable<IDataWriter>>>(
				tuple => base.WriteRecord(tuple.Item1, tuple.Item2, tuple.Item3),
				new ExecutionDataflowBlockOptions() { CancellationToken = this.CancellationToken });

			this.processRecordQueue.LinkTo(this.writeRecordQueue, new DataflowLinkOptions { PropagateCompletion = true }, tuple => tuple != null);
			this.processRecordQueue.LinkTo(DataflowBlock.NullTarget<Tuple<object, object, IEnumerable<IDataWriter>>>(), tuple => tuple == null);
		}

		protected virtual void WaitForCompletion()
		{
			this.processRecordQueue.Complete();

			try
			{
				this.writeRecordQueue.Completion.Wait();
			}
			catch (AggregateException ae)
			{
				ae.Flatten().Handle(ex => ex is TaskCanceledException);
			}

			if (this.CancellationToken != System.Threading.CancellationToken.None)
			{
				this.CancellationToken.ThrowIfCancellationRequested();
			}
		}

		protected override void ProcessRecord(object record, int recordIndex, out IEnumerable<IDataWriter> filteredDataWriters, out object mappedRecord, out object formattedRecord)
		{
			filteredDataWriters = null;
			mappedRecord = null;
			formattedRecord = null;

			this.processRecordQueue.Post(new Tuple<object, int>(record, recordIndex));
		}

		protected override void WriteRecord(object record, object formattedRecord, IEnumerable<IDataWriter> dataWriters)
		{
		}

		protected override void HandleUnreadRecord(int recordIndex, IEnumerable<FieldFailure> failures)
		{
			lock (this.RecordFailures)
			{
				base.HandleUnreadRecord(recordIndex, failures);
			}
		}

		protected override void HandleUnmappedRecord(int recordIndex, IEnumerable<FieldFailure> failures)
		{
			lock (this.RecordFailures)
			{
				base.HandleUnmappedRecord(recordIndex, failures);
			}

			throw new RecordFailedException();
		}

		protected override void HandleInvalidatedRecord(int recordIndex, IEnumerable<FieldFailure> failures)
		{
			lock (this.RecordFailures)
			{
				base.HandleInvalidatedRecord(recordIndex, failures);
			}

			throw new RecordFailedException();
		}

		#region Class - RecordFailedException

		protected class RecordFailedException : Exception
		{
		}

		#endregion
	}
}
