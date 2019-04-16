using System.Reflection;

namespace DotNetEtl
{
	public interface IFieldDisplayNameProvider
	{
		string GetFieldDisplayName(PropertyInfo property);
	}
}
