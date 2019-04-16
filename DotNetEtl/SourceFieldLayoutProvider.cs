using System.Reflection;

namespace DotNetEtl
{
	public class SourceFieldLayoutProvider : ISourceFieldLayoutProvider
	{
		public bool TryGetSourceFieldLayout(PropertyInfo property, object record, out int startIndex, out int length)
		{
			var fieldLayoutAttribute = property.GetCustomAttribute<SourceFieldLayoutAttribute>(true);

			startIndex = fieldLayoutAttribute?.StartIndex ?? -1;
			length = fieldLayoutAttribute?.Length ?? -1;

			return fieldLayoutAttribute != null;
		}
	}
}
