using System;

namespace DotNetEtl
{
	public interface IRecordFormatter
	{
		object Format(object record);
	}
}
