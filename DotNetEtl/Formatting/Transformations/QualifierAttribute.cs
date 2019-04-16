using System;

namespace DotNetEtl.Formatting.Transformations
{
	public class QualifierAttribute : TransformStringAttribute
	{
		private string qualifier;
		private string conditionalContains;

		public QualifierAttribute(string qualifier, string conditionalContains = null)
		{
			this.qualifier = qualifier;
			this.conditionalContains = conditionalContains;
		}

		public override bool CanTransformNullValue => true;

		public override string Transform(string value)
		{
			if (!String.IsNullOrEmpty(this.conditionalContains)
				&& (value == null || !value.Contains(this.conditionalContains)))
			{
				return value;
			}
			else
			{
				return String.Concat(this.qualifier, value, this.qualifier);
			}
		}
	}
}
