using System;

namespace DotNetEtl.Formatting
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class RightJustifyTextAttribute : JustifyTextAttribute
	{
		public RightJustifyTextAttribute(char paddingChar)
			: base(paddingChar)
		{
		}

		public RightJustifyTextAttribute()
			: base()
		{
		}
	}
}
