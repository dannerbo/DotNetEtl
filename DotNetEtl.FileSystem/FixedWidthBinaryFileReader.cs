using System.IO;

namespace DotNetEtl.FileSystem
{
	public class FixedWidthBinaryFileReader : FixedWidthBinaryStreamReader
	{
		public FixedWidthBinaryFileReader(string filePath, IRecordMapper recordMapper = null, FileShare fileShare = FileShare.None)
			: base(new FileStreamFactory(filePath, FileMode.Open, fileShare), recordMapper)
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
	}
}
