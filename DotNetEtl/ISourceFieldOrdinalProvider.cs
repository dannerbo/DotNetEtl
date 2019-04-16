using System.Reflection;

namespace DotNetEtl
{
	public interface ISourceFieldOrdinalProvider
	{
		bool TryGetSourceFieldOrdinal(PropertyInfo property, object record, out int ordinal);
	}
}
