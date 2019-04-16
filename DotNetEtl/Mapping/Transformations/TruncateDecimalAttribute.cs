using System;
using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class TruncateDecimalAttribute : TransformFieldAttribute
	{
		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			if (fieldValue is decimal)
			{
				return Math.Truncate((decimal)fieldValue);
			}
			else if (fieldValue is double)
			{
				return Math.Truncate((double)fieldValue);
			}

			throw new InvalidOperationException($"Value must be of type {typeof(decimal)} or {typeof(double)}.");
		}
	}
}
