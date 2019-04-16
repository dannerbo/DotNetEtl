namespace DotNetEtl.Formatting.Transformations
{
	public class TrimStartAttribute : TransformStringAttribute
	{
		public TrimStartAttribute(params char[] trimChars)
		{
			this.TrimChars = trimChars;
		}

		public TrimStartAttribute()
		{
		}

		public char[] TrimChars { get; private set; }

		public override string Transform(string value)
		{
			return this.TrimChars?.Length > 0
				? value.TrimStart(this.TrimChars)
				: value.TrimStart();
		}
	}
}
