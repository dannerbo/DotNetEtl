using System;

namespace DotNetEtl.Formatting.Transformations
{
	public class ReplaceTextAttribute : TransformStringAttribute
	{
		public ReplaceTextAttribute(string oldValue, string newValue)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		public string OldValue { get; private set; }
		public string NewValue { get; private set; }

		public override string Transform(string value)
		{
			return value.Replace(this.OldValue, this.NewValue);
		}
	}
}
