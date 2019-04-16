using System.Reflection;

namespace DotNetEtl
{
	public class DestinationFieldLayoutProvider : IDestinationFieldLayoutProvider
	{
		public bool TryGetDestinationFieldLayout(PropertyInfo property, object record, out int startIndex, out int length)
		{
			var fieldLayoutAttribute = property.GetCustomAttribute<DestinationFieldLayoutAttribute>(true);

			startIndex = fieldLayoutAttribute?.StartIndex ?? -1;
			length = fieldLayoutAttribute?.Length ?? -1;

			return fieldLayoutAttribute != null;
		}
	}
}
