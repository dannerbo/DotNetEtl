using System;
using System.Reflection;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public abstract class TransformFieldAttribute : Attribute
	{
		protected virtual bool CanTransformNullValue { get; } = false;
		
		public virtual void ApplyTransform(PropertyInfo property, object record)
		{
			var oldValue = property.GetValue(record);

			if (oldValue == null && !this.CanTransformNullValue)
			{
				return;
			}

			var newValue = this.GetTransformedValue(oldValue, property, record);

			if (oldValue != newValue)
			{
				property.SetValue(record, newValue);
			}
		}

		protected abstract object GetTransformedValue(object fieldValue, PropertyInfo property, object record);
	}
}
