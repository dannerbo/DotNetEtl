using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class TrimEndAttribute : TransformFieldAttribute
	{
		public TrimEndAttribute(params char[] trimChars)
		{
			this.TrimChars = trimChars;
		}

		public TrimEndAttribute()
		{
		}

		public char[] TrimChars { get; private set; }
		
		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			var text = (string)fieldValue;

			return this.TrimChars?.Length > 0
				? text.TrimEnd(this.TrimChars)
				: text.TrimEnd();
		}
	}
}
