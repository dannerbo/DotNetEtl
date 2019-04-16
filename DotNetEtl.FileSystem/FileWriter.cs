using System.IO;

namespace DotNetEtl.FileSystem
{
	public abstract class FileWriter : DataWriter
	{
		public FileWriter(string filePath, IRecordFormatter recordFormatter, FileMode fileMode = FileMode.CreateNew, FileShare fileShare = FileShare.None)
			: base(recordFormatter)
		{
			this.FilePath = filePath;
			this.FileMode = fileMode;
			this.FileShare = fileShare;
		}

		public FileWriter(string filePath, FileMode fileMode = FileMode.CreateNew, FileShare fileShare = FileShare.None)
		{
			this.FilePath = filePath;
			this.FileMode = fileMode;
			this.FileShare = fileShare;
		}

		public string FilePath { get; private set; }
		public FileMode FileMode { get; set; }
		public FileShare FileShare { get; set; }
		protected FileStream FileStream { get; private set; }

		public override void Open()
		{
			this.OpenFile();
		}

		protected virtual void OpenFile()
		{
			this.FileStream = File.Open(this.FilePath, this.FileMode, FileAccess.Write, this.FileShare);
		}

		public override void Close()
		{
			if (this.FileStream != null)
			{
				this.FileStream.Dispose();
				this.FileStream = null;
			}
		}

		public override void Commit()
		{
			this.Close();
		}

		public override void Rollback()
		{
			this.Close();

			File.Delete(this.FilePath);
		}
	}
}
