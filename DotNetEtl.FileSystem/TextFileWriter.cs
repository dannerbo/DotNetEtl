using System.IO;

namespace DotNetEtl.FileSystem
{
	public class TextFileWriter : FileWriter
	{
		public TextFileWriter(string filePath, IRecordFormatter recordFormatter, FileMode fileMode = FileMode.CreateNew, FileShare fileShare = FileShare.None)
			: base(filePath, recordFormatter, fileMode, fileShare)
		{
		}

		public TextFileWriter(string filePath, FileMode fileMode = FileMode.CreateNew, FileShare fileShare = FileShare.None)
			: base(filePath, fileMode, fileShare)
		{
		}
		
		protected StreamWriter StreamWriter { get; private set; }

		public override void Open()
		{
			base.Open();

			this.StreamWriter = new StreamWriter(this.FileStream);
		}

		public override void Close()
		{
			if (this.StreamWriter != null)
			{
				this.StreamWriter.Dispose();
				this.StreamWriter = null;
			}

			base.Close();
		}

		protected override void WriteRecordInternal(object record)
		{
			this.StreamWriter.WriteLine(record);
		}
	}
}
