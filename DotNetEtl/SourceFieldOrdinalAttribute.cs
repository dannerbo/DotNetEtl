using System;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class SourceFieldOrdinalAttribute : Attribute
	{
		public SourceFieldOrdinalAttribute(int ordinal)
		{
			this.Ordinal = ordinal;
		}

		public int Ordinal { get; private set; }
	}
}
