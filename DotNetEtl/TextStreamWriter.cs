using System.IO;

namespace DotNetEtl
{
	public class TextStreamWriter : StreamWriter
	{
		public TextStreamWriter(Stream stream, IRecordFormatter recordFormatter = null)
			: base(stream, recordFormatter)
		{
		}

		public TextStreamWriter(IStreamFactory streamFactory, IRecordFormatter recordFormatter = null)
			: base(streamFactory, recordFormatter)
		{
		}

		protected override void WriteRecordInternal(object record)
		{
			this.InternalStreamWriter.WriteLine(record);
		}
	}
}
