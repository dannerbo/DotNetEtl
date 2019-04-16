using System;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SourceFieldNameAttribute : Attribute
	{
		public SourceFieldNameAttribute(string fieldName)
		{
			this.FieldName = fieldName;
		}

		public string FieldName { get; private set; }
	}
}
