using System;
using System.IO;
using System.Threading;

namespace Drb.FileInjestion
{
	public class FileArchiver : IFileArchiver
	{
		public FileArchiver(string archivePath)
		{
			this.ArchivePath = archivePath;
		}

		protected string ArchivePath { get; private set; }

		public virtual void Archive(string filePath, CancellationToken cancellationToken)
		{
			var fileName = Path.GetFileName(filePath);
			var destinationFilePath = Path.Combine(this.ArchivePath, fileName);
			
			if (File.Exists(destinationFilePath))
			{
				destinationFilePath = this.GenerateNewFilePath(destinationFilePath);
			}

			File.Move(filePath, destinationFilePath);
		}

		protected virtual string GenerateNewFilePath(string filePath)
		{
			var fileName = Path.GetFileNameWithoutExtension(filePath);
			var fileExtension = Path.GetExtension(filePath);
			var directory = Path.GetDirectoryName(filePath);
			var counter = 0;

			while (File.Exists(filePath))
			{
				filePath = Path.Combine(directory, String.Format("{0} ({1})", fileName, ++counter));

				if (!String.IsNullOrEmpty(fileExtension))
				{
					filePath += fileExtension;
				}
			}

			return filePath;
		}
	}
}
