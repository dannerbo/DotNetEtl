using System;
using System.Reflection;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class ParseFieldAttribute : Attribute
	{
		public abstract bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage);
	}
}
