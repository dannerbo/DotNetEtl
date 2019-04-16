using System.Reflection;

namespace DotNetEtl.Mapping.Transformations
{
	public class DefaultAttribute : TransformFieldAttribute
	{
		public DefaultAttribute(object defaultValue)
		{
			this.DefaultValue = defaultValue;
		}

		public object DefaultValue { get; private set; }
		protected override bool CanTransformNullValue => true;

		protected override object GetTransformedValue(object fieldValue, PropertyInfo property, object record)
		{
			return fieldValue ?? this.DefaultValue;
		}
	}
}
