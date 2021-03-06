﻿using System;
using System.Reflection;

namespace DotNetEtl
{
	public class FieldParser : IFieldParser
	{
		public virtual bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
		{
			var parseFieldAttribute = property.GetCachedCustomAttribute<ParseFieldAttribute>();

			if (parseFieldAttribute != null)
			{
				return parseFieldAttribute.TryParse(property, fieldValue, out parsedFieldValue, out failureMessage);
			}

			try
			{
				var propertyType = property.PropertyType;

				if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					propertyType = Nullable.GetUnderlyingType(propertyType);
				}
				
				if (propertyType.IsEnum)
				{
					var fieldValueAsString = fieldValue as string;

					parsedFieldValue = fieldValueAsString != null
						? Enum.Parse(propertyType, fieldValueAsString)
						: Enum.ToObject(propertyType, fieldValue);
				}
				else
				{
					parsedFieldValue = Convert.ChangeType(fieldValue, propertyType);
				}
			}
			catch
			{
				parsedFieldValue = null;
				failureMessage = "Field is invalid.";

				return false;
			}

			failureMessage = null;

			return true;
		}
	}
}
