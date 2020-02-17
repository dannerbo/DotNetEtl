using System.Reflection;

namespace DotNetEtl
{
	public class DestinationFieldOrdinalProvider : IDestinationFieldOrdinalProvider
	{
		public bool TryGetDestinationFieldOrdinal(PropertyInfo property, object record, out int ordinal)
		{
			var ordinalAttribute = property.GetCachedCustomAttribute<DestinationFieldOrdinalAttribute>();

			ordinal = ordinalAttribute?.Ordinal ?? -1;

			return ordinalAttribute != null;
		}
	}
}
