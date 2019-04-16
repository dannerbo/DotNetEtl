using System;

namespace DotNetEtl
{
	public class RecordEventArgs : EventArgs
	{
		public RecordEventArgs(object record)
		{
			this.Record = record;
		}

		public object Record { get; private set; }
	}
}
