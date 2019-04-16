using System;

namespace DotNetEtl.Formatting
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class ToStringAttribute : Attribute
	{
		public abstract string ToString(object fieldValue);
	}
}
