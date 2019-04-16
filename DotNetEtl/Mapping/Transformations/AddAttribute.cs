using System;
using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class AddAttribute : TransformFieldAttribute
	{
		public AddAttribute(double amount)
		{
			this.Amount = amount;
		}

		public double Amount { get; private set; }

		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			var newValue = fieldValue?.GetType() == typeof(decimal) || fieldValue?.GetType() == typeof(decimal?)
				? (decimal)fieldValue + (decimal)this.Amount
				: (dynamic)fieldValue + (dynamic)this.Amount;

			return Convert.ChangeType(newValue, fieldValue.GetType());
		}
	}
}
