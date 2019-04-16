using System.Collections;

namespace DotNetEtl
{
	public class EnumerableReader : DataReader
	{
		private IEnumerator enumerator;

		public EnumerableReader(IEnumerable enumerable, IRecordMapper recordMapper = null)
			: base(recordMapper)
		{
			this.enumerator = enumerable.GetEnumerator();
		}

		protected override object ReadRecordInternal()
		{
			return this.enumerator.MoveNext()
				? this.enumerator.Current
				: null;
		}
	}
}
