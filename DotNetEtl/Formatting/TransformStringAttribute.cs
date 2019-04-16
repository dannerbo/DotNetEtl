using System;

namespace DotNetEtl.Formatting
{
	public abstract class TransformStringAttribute : Attribute
	{
		public virtual bool CanTransformNullValue { get; } = false;

		public abstract string Transform(string value);
	}
}
