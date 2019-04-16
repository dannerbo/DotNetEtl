using System;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DestinationFieldNameAttribute : Attribute
	{
		public DestinationFieldNameAttribute(string fieldName)
		{
			this.FieldName = fieldName;
		}

		public string FieldName { get; private set; }
	}
}
