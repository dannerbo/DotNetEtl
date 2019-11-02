using System.Reflection;

namespace DotNetEtl.Formatting
{
	public class TextRecordFieldFormatter : ITextRecordFieldFormatter
	{
		public virtual string Format(object propertyValue, PropertyInfo property)
		{
			var stringValue = (string)null;
			
			var toStringAttribute = property.GetCachedCustomAttribute<ToStringAttribute>();

			if (toStringAttribute != null)
			{
				stringValue = toStringAttribute.ToString(propertyValue);
			}
			else if (property.PropertyType == typeof(string))
			{
				stringValue = (string)propertyValue;
			}
			else
			{
				stringValue = propertyValue?.ToString();
			}
			
			var transformStringAttributes = property.GetCachedCustomAttributes<TransformStringAttribute>();

			foreach (var transformStringAttribute in transformStringAttributes)
			{
				if (stringValue != null || transformStringAttribute.CanTransformNullValue)
				{
					stringValue = transformStringAttribute.Transform(stringValue);
				}
			}

			return stringValue;
		}
	}
}
