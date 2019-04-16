using System.Reflection;

namespace DotNetEtl
{
	public interface IFieldParser
	{
		bool TryParse(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage);
	}
}
