using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class ToLowerAttribute : TransformFieldAttribute
	{
		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			return ((string)fieldValue).ToLower();
		}
	}
}
