using System.Reflection;

namespace DotNetEtl
{
	public class FieldTransformer : IFieldTransformer
	{
		public virtual void ApplyTransforms(PropertyInfo property, object record)
		{
			var transformations = property.GetCachedCustomAttributes<TransformFieldAttribute>();

			foreach (var transformation in transformations)
			{
				transformation.ApplyTransform(property, record);
			}
		}
	}
}
