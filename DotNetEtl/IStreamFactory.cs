using System.IO;

namespace DotNetEtl
{
	public interface IStreamFactory
	{
		Stream Create();
	}
}
