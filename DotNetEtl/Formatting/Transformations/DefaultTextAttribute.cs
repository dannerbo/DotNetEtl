namespace DotNetEtl.Formatting.Transformations
{
	public class DefaultTextAttribute : TransformStringAttribute
	{
		public DefaultTextAttribute(string defaultValue)
		{
			this.DefaultValue = defaultValue;
		}

		public string DefaultValue { get; private set; }
		public override bool CanTransformNullValue => true;

		public override string Transform(string value)
		{
			return value ?? this.DefaultValue;
		}
	}
}
