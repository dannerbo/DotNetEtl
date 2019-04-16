using System;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class DestinationFieldOrdinalAttribute : Attribute
	{
		public DestinationFieldOrdinalAttribute(int ordinal)
		{
			this.Ordinal = ordinal;
		}

		public int Ordinal { get; private set; }
	}
}
