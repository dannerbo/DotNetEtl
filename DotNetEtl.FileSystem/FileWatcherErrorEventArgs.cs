using System;

namespace DotNetEtl.FileSystem
{
	public class FileWatcherErrorEventArgs : ErrorEventArgs
	{
		public FileWatcherErrorEventArgs(Exception exception, string filePath = null)
			: base(exception)
		{
			this.FilePath = filePath;
		}

		public string FilePath { get; private set; }
	}
}
