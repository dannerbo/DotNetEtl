using System.Reflection;

namespace DotNetEtl
{
	public interface IDestinationFieldNameProvider
	{
		bool TryGetDestinationFieldName(PropertyInfo property, object record, out string fieldName);
	}
}
