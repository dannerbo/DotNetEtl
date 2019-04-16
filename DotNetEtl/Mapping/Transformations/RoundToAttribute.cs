using System;
using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class RoundToAttribute : TransformFieldAttribute
	{
		public RoundToAttribute(int decimals = 0, MidpointRounding midpointRounding = MidpointRounding.ToEven)
		{
			this.Decimals = decimals;
			this.MidpointRounding = midpointRounding;
		}

		public int Decimals { get; private set; }
		public MidpointRounding MidpointRounding { get; private set; }

		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			if (fieldValue is decimal)
			{
				return Math.Round((decimal)fieldValue, this.Decimals, this.MidpointRounding);
			}
			else if (fieldValue is double)
			{
				return Math.Round((double)fieldValue, this.Decimals, this.MidpointRounding);
			}

			throw new InvalidOperationException($"Value must be of type {typeof(decimal)} or {typeof(double)} to round.");
		}
	}
}
