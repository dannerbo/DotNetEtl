using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotNetEtl.Formatting
{
	public class DelimitedTextRecordFormatter : IRecordFormatter
	{
		public DelimitedTextRecordFormatter(string delimiter = ",", IDestinationFieldOrdinalProvider destinationFieldOrdinalProvider = null, ITextRecordFieldFormatter fieldFormatter = null)
		{
			this.Delimiter = delimiter;
			this.DestinationFieldOrdinalProvider = destinationFieldOrdinalProvider ?? new DestinationFieldOrdinalProvider();
			this.FieldFormatter = fieldFormatter ?? new TextRecordFieldFormatter();
		}

		public string Delimiter { get; set; }
		public string FieldQualifier { get; set; }
		public bool? IsFieldQualifierConditional { get; set; }
		protected IDestinationFieldOrdinalProvider DestinationFieldOrdinalProvider { get; private set; }
		protected ITextRecordFieldFormatter FieldFormatter { get; private set; }

		public virtual object Format(object record)
		{
			var fieldValues = new Dictionary<int, string>();

			var properties = record.GetType().GetCachedProperties();

			foreach (var property in properties)
			{
				if (this.TryGetDestinationFieldOrdinal(property, record, out int ordinal))
				{
					if (ordinal < 0)
					{
						throw new InvalidOperationException($"Encountered invalid ordinal of {ordinal}.");
					}

					var propertyValue = property.GetValue(record);
					var formattedPropertyValue = this.FormatField(propertyValue, property);

					try
					{
						fieldValues.Add(ordinal, formattedPropertyValue);
					}
					catch (ArgumentException)
					{
						throw new InvalidOperationException($"Record contains more than one field at ordinal {ordinal}.");
					}
				}
			}

			var formattedRecord = new StringBuilder();

			for (var ordinal = 0; ordinal < fieldValues.Count; ordinal++)
			{
				if (ordinal > 0)
				{
					formattedRecord.Append(this.Delimiter);
				}

				string fieldValue;

				if (!fieldValues.TryGetValue(ordinal, out fieldValue))
				{
					throw new InvalidOperationException($"Ordinal {ordinal} not found on record.");
				}

				if (!String.IsNullOrEmpty(this.FieldQualifier)
					&& !(this.IsFieldQualifierConditional.HasValue && this.IsFieldQualifierConditional.Value && (fieldValue == null || !fieldValue.Contains(this.Delimiter))))
				{
					formattedRecord.AppendFormat("{1}{0}{1}", fieldValue, this.FieldQualifier);
				}
				else
				{
					formattedRecord.Append(fieldValue);
				}
			}

			return formattedRecord.ToString();
		}

		protected virtual bool TryGetDestinationFieldOrdinal(PropertyInfo property, object record, out int ordinal)
		{
			return this.DestinationFieldOrdinalProvider.TryGetDestinationFieldOrdinal(property, record, out ordinal);
		}

		protected virtual string FormatField(object propertyValue, PropertyInfo property)
		{
			return this.FieldFormatter.Format(propertyValue, property);
		}
	}
}
