using System.Reflection;

namespace DotNetEtl
{
	public interface ISourceFieldLayoutProvider
	{
		bool TryGetSourceFieldLayout(PropertyInfo property, object record, out int startIndex, out int length);
	}
}
