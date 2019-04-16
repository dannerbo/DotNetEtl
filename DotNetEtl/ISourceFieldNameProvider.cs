using System.Reflection;

namespace DotNetEtl
{
	public interface ISourceFieldNameProvider
	{
		bool TryGetSourceFieldName(PropertyInfo property, object record, out string fieldName);
	}
}
