using System;

namespace DotNetEtl.Formatting.Transformations
{
	public class StringFormatAttribute : ToStringAttribute
	{
		public StringFormatAttribute(string format)
		{
			this.Format = format;
		}

		public string Format { get; private set; }

		public override string ToString(object fieldValue)
		{
			return String.Format($"{{0:{this.Format}}}", fieldValue);
		}
	}
}
