using System;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SourceRecordLengthAttribute : Attribute
	{
		public SourceRecordLengthAttribute(int length)
		{
			this.Length = length;
		}

		public int Length { get; private set; }
	}
}
