using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class ReplaceTextAttribute : TransformFieldAttribute
	{
		public ReplaceTextAttribute(string oldValue, string newValue)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}

		public string OldValue { get; private set; }
		public string NewValue { get; private set; }

		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			return ((string)fieldValue).Replace(this.OldValue, this.NewValue);
		}
	}
}
