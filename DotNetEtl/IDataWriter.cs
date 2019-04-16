using System;

namespace DotNetEtl
{
	public interface IDataWriter : IDisposable
	{
		void Open();
		void Close();
		void WriteRecord(object record);
		void Commit();
		void Rollback();
	}
}
