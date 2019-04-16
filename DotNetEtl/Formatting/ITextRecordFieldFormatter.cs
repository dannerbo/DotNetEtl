using System.Reflection;

namespace DotNetEtl.Formatting
{
	public interface ITextRecordFieldFormatter
	{
		string Format(object propertyValue, PropertyInfo property);
	}
}
