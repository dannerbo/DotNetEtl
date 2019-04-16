using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class PadLeftAttribute : TransformFieldAttribute
	{
		public PadLeftAttribute(int totalWidth, char paddingChar)
		{
			this.TotalWidth = totalWidth;
			this.PaddingChar = paddingChar;
		}

		public int TotalWidth { get; private set; }
		public char PaddingChar { get; private set; }

		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			return ((string)fieldValue).PadLeft(this.TotalWidth, this.PaddingChar);
		}
	}
}
