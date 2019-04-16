using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class TrimStartAttribute : TransformFieldAttribute
	{
		public TrimStartAttribute(params char[] trimChars)
		{
			this.TrimChars = trimChars;
		}

		public TrimStartAttribute()
		{
		}

		public char[] TrimChars { get; private set; }
		
		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			var text = (string)fieldValue;

			return this.TrimChars?.Length > 0
				? text.TrimStart(this.TrimChars)
				: text.TrimStart();
		}
	}
}
