namespace DotNetEtl.Formatting.Transformations
{
	public class TrimEndAttribute : TransformStringAttribute
	{
		public TrimEndAttribute(params char[] trimChars)
		{
			this.TrimChars = trimChars;
		}

		public TrimEndAttribute()
		{
		}

		public char[] TrimChars { get; private set; }

		public override string Transform(string value)
		{
			return this.TrimChars?.Length > 0
				? value.TrimEnd(this.TrimChars)
				: value.TrimEnd();
		}
	}
}
