using System.Reflection;

namespace DotNetEtl
{
	public class SourceFieldOrdinalProvider : ISourceFieldOrdinalProvider
	{
		public bool TryGetSourceFieldOrdinal(PropertyInfo property, object record, out int ordinal)
		{
			var ordinalAttribute = property.GetCachedCustomAttribute<SourceFieldOrdinalAttribute>();

			ordinal = ordinalAttribute?.Ordinal ?? -1;

			return ordinalAttribute != null;
		}
	}
}
