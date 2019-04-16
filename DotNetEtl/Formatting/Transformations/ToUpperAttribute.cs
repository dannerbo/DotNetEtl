namespace DotNetEtl.Formatting.Transformations
{
	public class ToUpperAttribute : TransformStringAttribute
	{
		public override string Transform(string value)
		{
			return value.ToUpper();
		}
	}
}
