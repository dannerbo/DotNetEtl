using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DotNetEtl
{
	public class FieldDisplayNameProvider : IFieldDisplayNameProvider
	{
		public string GetFieldDisplayName(PropertyInfo property)
		{
			var displayAttribute = property.GetCustomAttribute<DisplayAttribute>(true);

			return displayAttribute?.Name ?? property.Name;
		}
	}
}
