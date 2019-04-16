using System.Reflection;

namespace DotNetEtl
{
	public interface IDestinationFieldOrdinalProvider
	{
		bool TryGetDestinationFieldOrdinal(PropertyInfo property, object record, out int ordinal);
	}
}
