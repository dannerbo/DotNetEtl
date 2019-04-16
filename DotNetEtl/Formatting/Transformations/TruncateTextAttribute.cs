namespace DotNetEtl.Formatting.Transformations
{
	public class TruncateTextAttribute : TransformStringAttribute
	{
		public TruncateTextAttribute(int maxLength)
		{
			this.MaxLength = maxLength;
		}

		public int MaxLength { get; private set; }

		public override string Transform(string value)
		{
			return value.Length > this.MaxLength
				? value.Substring(0, this.MaxLength)
				: value;
		}
	}
}
