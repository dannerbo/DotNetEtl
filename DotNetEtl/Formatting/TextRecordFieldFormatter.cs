using System.Reflection;

namespace DotNetEtl.Formatting
{
	public class TextRecordFieldFormatter : ITextRecordFieldFormatter
	{
		public virtual string Format(object propertyValue, PropertyInfo property)
		{
			var stringValue = (string)null;
			var toStringAttribute = property.GetCustomAttribute<ToStringAttribute>(true);

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

			var transformStringAttributes = property.GetCustomAttributes<TransformStringAttribute>();

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
