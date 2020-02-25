using System.IO;

namespace DotNetEtl.FileSystem
{
	public class TextFileWriter : TextStreamWriter
	{
		public TextFileWriter(string filePath, IRecordFormatter recordFormatter = null, FileMode fileMode = FileMode.CreateNew, FileShare fileShare = FileShare.None)
			: base(new FileStreamFactory(filePath, fileMode, fileShare), recordFormatter)
		{
		}

		public string FilePath
		{
			get
			{
				return ((FileStreamFactory)this.StreamFactory).FilePath;
			}

			set
			{
				((FileStreamFactory)this.StreamFactory).FilePath = value;
			}
		}

		public FileMode FileMode
		{
			get
			{
				return ((FileStreamFactory)this.StreamFactory).FileMode;
			}

			set
			{
				((FileStreamFactory)this.StreamFactory).FileMode = value;
			}
		}

		public FileShare FileShare
		{
			get
			{
				return ((FileStreamFactory)this.StreamFactory).FileShare;
			}

			set
			{
				((FileStreamFactory)this.StreamFactory).FileShare = value;
			}
		}

		public override void Rollback()
		{
			this.Close();

			File.Delete(this.FilePath);
		}
	}
}
