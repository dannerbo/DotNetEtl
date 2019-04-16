using System.Reflection;

namespace DotNetEtl
{
	public interface IFieldTransformer
	{
		void ApplyTransforms(PropertyInfo property, object record);
	}
}
