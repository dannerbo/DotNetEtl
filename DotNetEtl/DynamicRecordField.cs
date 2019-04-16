using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DotNetEtl
{
	public class DynamicRecordField : IDynamicRecordField
	{
		public string Name { get; set; }
		public Type DotNetDataType { get; set; }
		public int? SourceFieldOrdinal { get; set; }
		public int? SourceFieldStartIndex { get; set; }
		public int? SourceFieldLength { get; set; }
		public string SourceFieldName { get; set; }
		public int? DestinationFieldOrdinal { get; set; }
		public int? DestinationFieldStartIndex { get; set; }
		public int? DestinationFieldLength { get; set; }
		public string DestinationFieldName { get; set; }

		public virtual IEnumerable<CustomAttributeBuilder> GetCustomAttributes()
		{
			var attributes = new List<CustomAttributeBuilder>();

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

			return attributes;
		}
	}
}
