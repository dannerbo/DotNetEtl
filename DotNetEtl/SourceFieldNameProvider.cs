using System.Reflection;

namespace DotNetEtl
{
	public class SourceFieldNameProvider : ISourceFieldNameProvider
	{
		public bool TryGetSourceFieldName(PropertyInfo property, object record, out string fieldName)
		{
			var fieldNameAttribute = property.GetCustomAttribute<SourceFieldNameAttribute>(true);

			fieldName = fieldNameAttribute?.FieldName;

			return fieldNameAttribute != null;
		}
	}
}
