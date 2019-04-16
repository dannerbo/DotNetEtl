using System;

namespace DotNetEtl.Formatting.Transformations
{
	public class PadRightAttribute : TransformStringAttribute
	{
		private bool padNullValues;

		public PadRightAttribute(int totalWidth, char paddingChar, bool padNullValues = false)
		{
			this.TotalWidth = totalWidth;
			this.PaddingChar = paddingChar;
			this.padNullValues = padNullValues;
		}

		public override bool CanTransformNullValue => this.padNullValues;
		public int TotalWidth { get; private set; }
		public char PaddingChar { get; private set; }

		public override string Transform(string value)
		{
			return (value ?? String.Empty).PadRight(this.TotalWidth, this.PaddingChar);
		}
	}
}
