using System.IO;

namespace DotNetEtl.FileSystem
{
	public static class FileHelper
	{
		public static bool IsFileLocked(string filePath)
		{
			try
			{
				using (File.OpenRead(filePath))
				{
				}
			}
			catch (FileNotFoundException)
			{
				throw;
			}
			catch (IOException)
			{
				return true;
			}

			return false;
		}
	}
}
