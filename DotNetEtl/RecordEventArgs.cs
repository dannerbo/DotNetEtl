using System;

namespace DotNetEtl
{
	public class RecordEventArgs : EventArgs
	{
		public RecordEventArgs(int recordIndex, object record)
		{
			this.RecordIndex = recordIndex;
			this.Record = record;
		}

		public int RecordIndex { get; private set; }
		public object Record { get; private set; }
	}
}
