using System.Collections.Generic;
using System.Threading;

namespace DotNetEtl
{
	public interface IDataImport
	{
		bool CanCommitWithRecordFailures { get; set; }
		IDataSource DataSource { get; }

		bool TryRun(out IEnumerable<RecordFailure> failures);
		bool TryRun(CancellationToken cancellationToken, out IEnumerable<RecordFailure> failures);
		void Run();
		void Run(CancellationToken cancellationToken);
	}
}
