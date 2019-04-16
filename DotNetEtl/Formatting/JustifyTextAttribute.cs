using System;

namespace DotNetEtl.Formatting
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public abstract class JustifyTextAttribute : Attribute
	{
		public JustifyTextAttribute(char paddingChar)
		{
			this.PaddingChar = paddingChar;
		}

		public JustifyTextAttribute()
		{
		}

		public char? PaddingChar { get; private set; } = null;
	}
}
