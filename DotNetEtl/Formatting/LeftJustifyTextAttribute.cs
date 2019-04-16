using System;

namespace DotNetEtl.Formatting
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class LeftJustifyTextAttribute : JustifyTextAttribute
	{
		public LeftJustifyTextAttribute(char paddingChar)
			: base(paddingChar)
		{
		}

		public LeftJustifyTextAttribute()
			: base()
		{
		}
	}
}
