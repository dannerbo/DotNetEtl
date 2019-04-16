using System.Reflection;

namespace DotNetEtl.Formatting
{
	public interface ITextRecordFieldPaddingStrategy
	{
		string Pad(string fieldValue, int fieldLength, PropertyInfo property);
	}
}
