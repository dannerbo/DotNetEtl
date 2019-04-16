using System.IO;

namespace DotNetEtl.FileSystem
{
	public class TextFileReader : FileReader
	{
		public TextFileReader(string filePath, IRecordMapper recordMapper = null, FileShare fileShare = FileShare.None)
			: base(filePath, recordMapper, fileShare)
		{
		}

		public int? HeaderRowCount { get; set; }
		protected StreamReader StreamReader { get; private set; }

		public override void Open()
		{
			base.Open();

			this.StreamReader = new StreamReader(this.FileStream);
		}

		public override void Close()
		{
			if (this.StreamReader != null)
			{
				this.StreamReader.Dispose();
				this.StreamReader = null;
			}

			base.Close();
		}

		protected override object ReadRecordInternal()
		{
			if (this.HeaderRowCount.HasValue && this.StreamReader.BaseStream.Position == 0)
			{
				this.ReadHeader();
			}

			var line = this.ReadLine();

			return line != null && !this.IsFooter(line)
				? line
				: null;
		}
		
		protected virtual void ReadHeader()
		{
			for (var i = 0; i < this.HeaderRowCount.Value; i++)
			{
				this.ReadLine();
			}
		}

		protected virtual bool IsFooter(string line)
		{
			return false;
		}

		protected virtual string ReadLine()
		{
			return this.StreamReader.ReadLine();
		}
	}
}
