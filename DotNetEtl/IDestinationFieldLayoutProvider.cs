using System.Reflection;

namespace DotNetEtl
{
	public interface IDestinationFieldLayoutProvider
	{
		bool TryGetDestinationFieldLayout(PropertyInfo property, object record, out int startIndex, out int length);
	}
}
