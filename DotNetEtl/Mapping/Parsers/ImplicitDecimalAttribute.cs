using System;
using System.Reflection;

namespace DotNetEtl.Mapping.Parsers
{
	public class ImplicitDecimalAttribute : ParseFieldAttribute
	{
		public ImplicitDecimalAttribute(int decimals)
		{
			this.Decimals = decimals;
		}

		public int Decimals { get; private set; }

		public override bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
		{
			parsedFieldValue = null;

			var fieldValueAsString = ((string)fieldValue).Trim();

			if (fieldValueAsString.Contains("."))
			{
				failureMessage = "Value contains a decimal.";

				return false;
			}

			try
			{
				var fieldValueAsStringWithDecimal = fieldValueAsString.Insert(fieldValueAsString.Length - this.Decimals, ".");
				var newValue = Double.Parse(fieldValueAsStringWithDecimal);
				var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
				
				parsedFieldValue = Convert.ChangeType(newValue, targetType);
			}
			catch (ArgumentOutOfRangeException)
			{
				failureMessage = "Field has incorrect number of digits.";

				return false;
			}
			catch (FormatException)
			{
				failureMessage = "Field is not in the correct format.";

				return false;
			}

			failureMessage = null;

			return true;
		}
	}
}
