using System.IO;

namespace DotNetEtl
{
	public class TextStreamReader : StreamReader
	{
		public TextStreamReader(Stream stream, IRecordMapper recordMapper = null)
			: base(stream, recordMapper)
		{
		}

		public TextStreamReader(IStreamFactory streamFactory, IRecordMapper recordMapper = null)
			: base(streamFactory, recordMapper)
		{
		}

		public int HeaderRowCount { get; set; }

		protected override object ReadRecordInternal()
		{
			this.ReadPastHeaderIfNeeded();

			var line = this.ReadLine();

			return line != null && !this.IsFooter(line)
				? line
				: null;
		}

		protected virtual string ReadLine()
		{
			return this.InternalStreamReader.ReadLine();
		}

		protected virtual bool IsFooter(string line)
		{
			return false;
		}

		private void ReadPastHeaderIfNeeded()
		{
			if (this.HeaderRowCount > 0 && this.InternalStreamReader.BaseStream.Position == 0)
			{
				for (var i = 0; i < this.HeaderRowCount; i++)
				{
					this.InternalStreamReader.ReadLine();
				}
			}
		}
	}
}
