using System;
using System.IO;

namespace DotNetEtl.FileSystem
{
	public class FileStreamFactory : IStreamFactory
	{
		public FileStreamFactory(string filePath, FileMode fileMode, FileAccess fileAccess, FileShare fileShare = FileShare.None)
		{
			this.FilePath = filePath;
			this.FileMode = fileMode;
			this.FileAccess = fileAccess;
			this.FileShare = fileShare;
		}

		public FileStreamFactory()
		{
		}

		public string FilePath { get; set; }
		public FileMode FileMode { get; set; }
		public FileAccess FileAccess { get; set; }
		public FileShare FileShare { get; set; }

		public virtual Stream Create()
		{
			this.ThrowIfFilePathIsNullOrEmpty();

			return File.Open(this.FilePath, this.FileMode, this.FileAccess, this.FileShare);
		}

		private void ThrowIfFilePathIsNullOrEmpty()
		{
			if (String.IsNullOrEmpty(this.FilePath))
			{
				throw new InvalidOperationException("File path was not provided.");
			}
		}
	}
}
