using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DotNetEtl.SqlServer
{
	public class DatabaseWriterCommandFactory : IDatabaseWriterCommandFactory
	{
		public DatabaseWriterCommandFactory(IDatabaseWriterCommandParameterProvider commandParameterProvider, CommandType commandType, string commandText)
			: this(commandParameterProvider, commandText)
		{
			this.CommandType = commandType;
		}

		public DatabaseWriterCommandFactory(IDatabaseWriterCommandParameterProvider commandParameterProvider, string commandText)
		{
			this.CommandParameterProvider = commandParameterProvider;
			this.CommandText = commandText;
		}

		public DatabaseWriterCommandFactory(CommandType commandType, string commandText)
			: this(new DatabaseWriterCommandParameterProvider(), commandText)
		{
			this.CommandType = commandType;
		}

		public DatabaseWriterCommandFactory(IEnumerable<SqlParameter> staticCommandParameters, CommandType commandType, string commandText)
			: this(new DatabaseWriterCommandParameterProvider(staticCommandParameters), commandText)
		{
			this.CommandType = commandType;
		}

		public DatabaseWriterCommandFactory(IDatabaseWriterCommandParameterProvider commandParameterProvider)
		{
			this.CommandParameterProvider = commandParameterProvider;
		}

		public DatabaseWriterCommandFactory()
			: this(new DatabaseWriterCommandParameterProvider())
		{
		}

		public string CommandText { get; set; }
		public CommandType CommandType { get; set; } = CommandType.StoredProcedure;
		public TimeSpan CommandTimeout { get; set; } = TimeSpan.FromSeconds(30);
		protected IDatabaseWriterCommandParameterProvider CommandParameterProvider { get; private set; }

		public virtual SqlCommand Create(object record)
		{
			var command = new SqlCommand(this.CommandText)
			{
				CommandType = this.CommandType,
				CommandTimeout = (int)this.CommandTimeout.TotalMilliseconds
			};
			
			var parameters = this.CommandParameterProvider.GetParameters(record);

			if (parameters.Count() > 0)
			{
				command.Parameters.AddRange(parameters.ToArray());
			}

			return command;
		}
	}
}
