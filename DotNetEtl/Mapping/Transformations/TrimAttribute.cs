using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class TrimAttribute : TransformFieldAttribute
	{
		public TrimAttribute(params char[] trimChars)
		{
			this.TrimChars = trimChars;
		}

		public TrimAttribute()
		{
		}

		public char[] TrimChars { get; private set; }
		
		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			var text = (string)fieldValue;

			return this.TrimChars?.Length > 0
				? text.Trim(this.TrimChars)
				: text.Trim();
		}
	}
}
