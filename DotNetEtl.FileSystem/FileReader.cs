using System.IO;

namespace DotNetEtl.FileSystem
{
	public abstract class FileReader : DataReader
	{
		public FileReader(string filePath, IRecordMapper recordMapper = null, FileShare fileShare = FileShare.None)
			: base(recordMapper)
		{
			this.FilePath = filePath;
			this.FileShare = fileShare;
		}

		public string FilePath { get; private set; }
		public FileShare FileShare { get; set; }
		protected FileStream FileStream { get; private set; }

		public override void Open()
		{
			this.OpenFile();
		}

		public override void Close()
		{
			if (this.FileStream != null)
			{
				this.FileStream.Dispose();
				this.FileStream = null;
			}
		}

		protected virtual void OpenFile()
		{
			this.FileStream = File.Open(this.FilePath, FileMode.Open, FileAccess.Read, this.FileShare);
		}
	}
}
