using System;
using System.Reflection;

namespace DotNetEtl.Mapping.Parsers
{
	public class BitAttribute : ParseFieldAttribute
	{
		public object TrueValue { get; set; } = true;
		public object FalseValue { get; set; } = false;

		public override bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
		{
			try
			{
				var bitValue = (int)Convert.ChangeType(fieldValue, typeof(int));

				if (bitValue == 0 || bitValue == 1)
				{
					parsedFieldValue = bitValue == 1 ? this.TrueValue : this.FalseValue;
					failureMessage = null;

					return true;
				}
			}
			catch
			{
			}

			parsedFieldValue = null;
			failureMessage = "Field is not equal to one or zero.";

			return false;
		}
	}
}
