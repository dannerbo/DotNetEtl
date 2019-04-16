using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class TruncateTextAttribute : TransformFieldAttribute
	{
		public TruncateTextAttribute(int maxLength)
		{
			this.MaxLength = maxLength;
		}

		public int MaxLength { get; private set; }
		
		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			var text = (string)fieldValue;

			return text.Length > this.MaxLength
				? text.Substring(0, this.MaxLength)
				: text;
		}
	}
}
