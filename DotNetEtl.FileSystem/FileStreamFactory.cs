using System;
using System.IO;

namespace DotNetEtl.FileSystem
{
	public class FileStreamFactory : IStreamFactory
	{
		public FileStreamFactory(string filePath, FileShare fileShare = FileShare.None)
		{
			this.FilePath = filePath;
			this.FileShare = fileShare;
		}

		public FileStreamFactory()
		{
		}

		public string FilePath { get; set; }
		public FileShare FileShare { get; set; }

		public virtual Stream Create()
		{
			this.ThrowIfFilePathIsNullOrEmpty();

			return File.Open(this.FilePath, FileMode.Open, FileAccess.Read, this.FileShare);
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
