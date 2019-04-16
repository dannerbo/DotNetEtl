using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace DotNetEtl
{
	public class DataAnnotatedDynamicRecordField : DynamicRecordField
	{
		public DataType? DataType { get; set; }
		public Type EnumDataType { get; set; }
		public Type RangeDataType { get; set; }
		public string RangeMinimum { get; set; }
		public string RangeMaximum { get; set; }
		public string RegularExpression { get; set; }
		public bool? Required { get; set; }
		public int? MinLength { get; set; }
		public int? MaxLength { get; set; }
		public Type CustomValidatorType { get; set; }
		public string CustomValidatorMethod { get; set; }
		
		public override IEnumerable<CustomAttributeBuilder> GetCustomAttributes()
		{
			var attributes = new List<CustomAttributeBuilder>(base.GetCustomAttributes());

			if (this.SourceFieldOrdinal != null)
			{
				var constructor = typeof(SourceFieldOrdinalAttribute).GetConstructor(new Type[] { typeof(int) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.SourceFieldOrdinal.Value });

				attributes.Add(attribute);
			}

			if (this.SourceFieldStartIndex != null && this.SourceFieldLength != null)
			{
				var constructor = typeof(SourceFieldLayoutAttribute).GetConstructor(new Type[] { typeof(int), typeof(int) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.SourceFieldStartIndex.Value, this.SourceFieldLength.Value });

				attributes.Add(attribute);
			}

			if (this.SourceFieldName != null)
			{
				var constructor = typeof(SourceFieldNameAttribute).GetConstructor(new Type[] { typeof(string) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.SourceFieldName });

				attributes.Add(attribute);
			}

			if (this.DestinationFieldOrdinal != null)
			{
				var constructor = typeof(DestinationFieldOrdinalAttribute).GetConstructor(new Type[] { typeof(int) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.DestinationFieldOrdinal.Value });

				attributes.Add(attribute);
			}

			if (this.DestinationFieldStartIndex != null && this.DestinationFieldLength != null)
			{
				var constructor = typeof(DestinationFieldLayoutAttribute).GetConstructor(new Type[] { typeof(int), typeof(int) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.DestinationFieldStartIndex.Value, this.DestinationFieldLength.Value });

				attributes.Add(attribute);
			}

			if (this.DestinationFieldName != null)
			{
				var constructor = typeof(DestinationFieldNameAttribute).GetConstructor(new Type[] { typeof(string) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.DestinationFieldName });

				attributes.Add(attribute);
			}

			if (this.DataType != null)
			{
				var constructor = typeof(DataTypeAttribute).GetConstructor(new Type[] { typeof(DataType) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.DataType.Value });

				attributes.Add(attribute);
			}

			if (this.EnumDataType != null)
			{
				var constructor = typeof(EnumDataTypeAttribute).GetConstructor(new Type[] { typeof(Type) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.EnumDataType });

				attributes.Add(attribute);
			}

			if (this.RangeDataType != null && this.RangeMinimum != null && this.RangeMaximum != null)
			{
				var constructor = typeof(RangeAttribute).GetConstructor(new Type[] { typeof(Type), typeof(string), typeof(string) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.RangeDataType, this.RangeMinimum, this.RangeMaximum });

				attributes.Add(attribute);
			}

			if (this.RegularExpression != null)
			{
				var constructor = typeof(RegularExpressionAttribute).GetConstructor(new Type[] { typeof(string) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.RegularExpression });

				attributes.Add(attribute);
			}

			if (this.Required != null && this.Required.Value == true)
			{
				var constructor = typeof(RequiredAttribute).GetConstructor(Type.EmptyTypes);
				var attribute = new CustomAttributeBuilder(constructor, new object[] { });

				attributes.Add(attribute);
			}

			if (this.MinLength != null)
			{
				var constructor = typeof(MinLengthAttribute).GetConstructor(new Type[] { typeof(int) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.MinLength.Value });

				attributes.Add(attribute);
			}

			if (this.MaxLength != null)
			{
				var constructor = typeof(MaxLengthAttribute).GetConstructor(new Type[] { typeof(int) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.MaxLength.Value });

				attributes.Add(attribute);
			}

			if (this.CustomValidatorType != null && this.CustomValidatorMethod != null)
			{
				var constructor = typeof(CustomValidationAttribute).GetConstructor(new Type[] { typeof(Type), typeof(string) });
				var attribute = new CustomAttributeBuilder(constructor, new object[] { this.CustomValidatorType, this.CustomValidatorMethod });

				attributes.Add(attribute);
			}

			return attributes;
		}
	}
}
