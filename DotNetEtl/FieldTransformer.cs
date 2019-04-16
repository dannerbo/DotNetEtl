using System.Reflection;

namespace DotNetEtl
{
	public class FieldTransformer : IFieldTransformer
	{
		public virtual void ApplyTransforms(PropertyInfo property, object record)
		{
			var transformations = property.GetCustomAttributes<TransformFieldAttribute>(true);

			foreach (var transformation in transformations)
			{
				transformation.ApplyTransform(property, record);
			}
		}
	}
}
