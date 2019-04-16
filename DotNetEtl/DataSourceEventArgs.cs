using System;

namespace DotNetEtl
{
	public class DataSourceEventArgs : EventArgs
	{
		public DataSourceEventArgs(IDataSource dataSource)
		{
			this.DataSource = dataSource;
		}
		
		public IDataSource DataSource { get; private set; }
	}
}
