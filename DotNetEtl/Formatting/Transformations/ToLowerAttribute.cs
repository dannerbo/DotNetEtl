namespace DotNetEtl.Formatting.Transformations
{
	public class ToLowerAttribute : TransformStringAttribute
	{
		public override string Transform(string value)
		{
			return value.ToLower();
		}
	}
}
