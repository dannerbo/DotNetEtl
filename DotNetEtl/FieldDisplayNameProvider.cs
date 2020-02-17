using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DotNetEtl
{
	public class FieldDisplayNameProvider : IFieldDisplayNameProvider
	{
		public string GetFieldDisplayName(PropertyInfo property)
		{
			var displayAttribute = property.GetCachedCustomAttribute<DisplayAttribute>();

			return displayAttribute?.Name ?? property.Name;
		}
	}
}
