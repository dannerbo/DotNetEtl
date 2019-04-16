using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class PadRightAttribute : TransformFieldAttribute
	{
		public PadRightAttribute(int totalWidth, char paddingChar)
		{
			this.TotalWidth = totalWidth;
			this.PaddingChar = paddingChar;
		}

		public int TotalWidth { get; private set; }
		public char PaddingChar { get; private set; }

		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			return ((string)fieldValue).PadRight(this.TotalWidth, this.PaddingChar);
		}
	}
}
