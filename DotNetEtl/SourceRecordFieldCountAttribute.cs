using System;

namespace DotNetEtl
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SourceRecordFieldCountAttribute : Attribute
	{
		public SourceRecordFieldCountAttribute(int count)
		{
			this.Count = count;
		}

		public int Count { get; private set; }
	}
}
