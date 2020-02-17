﻿using System.Reflection;

namespace DotNetEtl
{
	public class DestinationFieldNameProvider : IDestinationFieldNameProvider
	{
		public bool TryGetDestinationFieldName(PropertyInfo property, object record, out string fieldName)
		{
			var fieldNameAttribute = property.GetCachedCustomAttribute<DestinationFieldNameAttribute>();

			fieldName = fieldNameAttribute?.FieldName;

			return fieldNameAttribute != null;
		}
	}
}
