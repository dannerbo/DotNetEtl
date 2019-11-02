using System;
using System.Reflection;

namespace DotNetEtl.Formatting
{
	public class TextRecordFieldPaddingStrategy : ITextRecordFieldPaddingStrategy
	{		
		public char TextPaddingChar { get; set; } = ' ';
		public char NumberPaddingChar { get; set; } = ' ';
		public char DateTimePaddingChar { get; set; } = ' ';
		public bool LeftJustifiyText { get; set; } = true;
		public bool RightJustifyNumbers { get; set; } = true;
		public bool RightJustifyDateTimes { get; set; } = true;

		public string Pad(string fieldValue, int fieldLength, PropertyInfo property)
		{
			var justifyAttribute = property.GetCachedCustomAttribute<JustifyTextAttribute>();

			if (justifyAttribute != null
				&& !(justifyAttribute is LeftJustifyTextAttribute || justifyAttribute is RightJustifyTextAttribute))
			{
				throw new InvalidOperationException("Invalid text justification attribute provided.");
			}
			
			if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
			{
				if (justifyAttribute != null)
				{
					return justifyAttribute is LeftJustifyTextAttribute
						? fieldValue.PadRight(fieldLength, justifyAttribute.PaddingChar ?? this.DateTimePaddingChar) 
						: fieldValue.PadLeft(fieldLength, justifyAttribute.PaddingChar ?? this.DateTimePaddingChar);
				}
				else
				{
					return this.RightJustifyDateTimes
						? fieldValue.PadLeft(fieldLength, this.DateTimePaddingChar)
						: fieldValue.PadRight(fieldLength, this.DateTimePaddingChar);
				}
			}
			else if (TypeHelper.IsNumeric(property.PropertyType))
			{
				if (justifyAttribute != null)
				{
					return justifyAttribute is LeftJustifyTextAttribute
						? fieldValue.PadRight(fieldLength, justifyAttribute.PaddingChar ?? this.NumberPaddingChar)
						: fieldValue.PadLeft(fieldLength, justifyAttribute.PaddingChar ?? this.NumberPaddingChar);
				}
				else
				{
					return this.RightJustifyNumbers
						? fieldValue.PadLeft(fieldLength, this.NumberPaddingChar)
						: fieldValue.PadRight(fieldLength, this.NumberPaddingChar);
				}
			}
			else
			{
				if (justifyAttribute != null)
				{
					return justifyAttribute is LeftJustifyTextAttribute
						? fieldValue.PadRight(fieldLength, justifyAttribute.PaddingChar ?? this.TextPaddingChar)
						: fieldValue.PadLeft(fieldLength, justifyAttribute.PaddingChar ?? this.TextPaddingChar);
				}
				else
				{
					return this.LeftJustifiyText
						? fieldValue.PadRight(fieldLength, this.TextPaddingChar)
						: fieldValue.PadLeft(fieldLength, this.TextPaddingChar);
				}
			}
		}
	}
}
