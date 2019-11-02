using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DotNetEtl.Formatting
{
	public class FixedWidthTextRecordFormatter : IRecordFormatter
	{
		public FixedWidthTextRecordFormatter(IDestinationFieldLayoutProvider destinationFieldLayoutProvider = null, ITextRecordFieldFormatter fieldFormatter = null, ITextRecordFieldPaddingStrategy fieldPaddingStrategy = null)
		{
			this.DestinationFieldLayoutProvider = destinationFieldLayoutProvider ?? new DestinationFieldLayoutProvider();
			this.FieldFormatter = fieldFormatter ?? new TextRecordFieldFormatter();
			this.FieldPaddingStrategy = fieldPaddingStrategy ?? new TextRecordFieldPaddingStrategy();
		}

		protected IDestinationFieldLayoutProvider DestinationFieldLayoutProvider { get; private set; }
		protected ITextRecordFieldFormatter FieldFormatter { get; private set; }
		protected ITextRecordFieldPaddingStrategy FieldPaddingStrategy { get; private set; }

		public virtual object Format(object record)
		{
			Dictionary<int, string> fieldValues;
			Dictionary<int, int> fieldLengths;
			Dictionary<int, PropertyInfo> fieldProperties;

			this.GetFieldAttributes(record, out fieldValues, out fieldLengths, out fieldProperties);

			var formattedRecord = new StringBuilder();
			var startIndex = 0;
			
			for (int i = 0; i < fieldValues.Count; i++)
			{
				if (!fieldValues.TryGetValue(startIndex, out string fieldValue))
				{
					throw new InvalidOperationException($"Expected a field with start index of {startIndex} but did not find one.");
				}

				var fieldLength = fieldLengths[startIndex];

				if (fieldValue.Length > fieldLength)
				{
					throw new InvalidOperationException($"Field at start index {startIndex} has a value length ({fieldValue.Length}) that exceeds the field definition length of {fieldLength}.");
				}
				else if (fieldValue.Length < fieldLength)
				{
					fieldValue = this.FieldPaddingStrategy.Pad(fieldValue, fieldLength, fieldProperties[startIndex]);
				}

				formattedRecord.Append(fieldValue);

				startIndex += fieldLength;
			}

			return formattedRecord.ToString();
		}

		protected virtual bool TryGetDestinationFieldLayout(PropertyInfo property, object record, out int startIndex, out int length)
		{
			return this.DestinationFieldLayoutProvider.TryGetDestinationFieldLayout(property, record, out startIndex, out length);
		}

		protected virtual string FormatField(object propertyValue, PropertyInfo property)
		{
			return this.FieldFormatter.Format(propertyValue, property);
		}

		protected void GetFieldAttributes(
			object record,
			out Dictionary<int, string> fieldValues,
			out Dictionary<int, int> fieldLengths,
			out Dictionary<int, PropertyInfo> fieldProperties)
		{
			fieldValues = new Dictionary<int, string>();
			fieldLengths = new Dictionary<int, int>();
			fieldProperties = new Dictionary<int, PropertyInfo>();

			var properties = record.GetType().GetCachedProperties();

			foreach (var property in properties)
			{
				if (this.TryGetDestinationFieldLayout(property, record, out int startIndex, out int length))
				{
					if (startIndex < 0)
					{
						throw new InvalidOperationException($"Encountered invalid start index of {startIndex}.");
					}

					var propertyValue = property.GetValue(record);

					try
					{
						var formattedPropertyValue = this.FormatField(propertyValue, property);

						fieldValues.Add(startIndex, formattedPropertyValue ?? String.Empty);
						fieldLengths.Add(startIndex, length);
						fieldProperties.Add(startIndex, property);
					}
					catch (ArgumentException)
					{
						throw new InvalidOperationException($"Record contains more than one field at start index {startIndex}.");
					}
				}
			}
		}
	}
}
