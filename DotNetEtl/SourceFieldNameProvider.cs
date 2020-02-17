using System.Reflection;

namespace DotNetEtl
{
	public class SourceFieldNameProvider : ISourceFieldNameProvider
	{
		public bool TryGetSourceFieldName(PropertyInfo property, object record, out string fieldName)
		{
			var fieldNameAttribute = property.GetCachedCustomAttribute<SourceFieldNameAttribute>();

			fieldName = fieldNameAttribute?.FieldName;

			return fieldNameAttribute != null;
		}
	}
}
