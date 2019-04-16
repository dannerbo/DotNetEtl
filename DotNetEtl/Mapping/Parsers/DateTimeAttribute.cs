using System;
using System.Globalization;
using System.Reflection;

namespace DotNetEtl.Mapping.Parsers
{
	public class DateTimeAttribute : ParseFieldAttribute
	{
		public DateTimeAttribute(string format)
		{
			this.Format = format;
		}

		public string Format { get; private set; }
		public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.None;

		public override bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
		{
			if (!DateTime.TryParseExact((string)fieldValue, this.Format, CultureInfo.InvariantCulture, this.DateTimeStyles, out DateTime parsedDateTime))
			{
				parsedFieldValue = null;
				failureMessage = "Field is not a valid date/time.";

				return false;
			}

			failureMessage = null;
			parsedFieldValue = parsedDateTime;

			return true;
		}
	}
}
