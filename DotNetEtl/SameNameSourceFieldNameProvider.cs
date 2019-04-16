using System.Reflection;

namespace DotNetEtl
{
	public class SameNameSourceFieldNameProvider : ISourceFieldNameProvider
	{
		public bool TryGetSourceFieldName(PropertyInfo property, object record, out string fieldName)
		{
			fieldName = property.Name;

			return true;
		}
	}
}
