namespace DotNetEtl.Formatting.Transformations
{
	public class TrimAttribute : TransformStringAttribute
	{
		public TrimAttribute(params char[] trimChars)
		{
			this.TrimChars = trimChars;
		}

		public TrimAttribute()
		{
		}

		public char[] TrimChars { get; private set; }

		public override string Transform(string value)
		{
			return this.TrimChars?.Length > 0
				? value.Trim(this.TrimChars)
				: value.Trim();
		}
	}
}
