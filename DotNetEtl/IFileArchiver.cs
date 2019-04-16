using System.Threading;

namespace Drb.FileInjestion
{
	public interface IFileArchiver
	{
		void Archive(string filePath, CancellationToken cancellationToken);
	}
}
