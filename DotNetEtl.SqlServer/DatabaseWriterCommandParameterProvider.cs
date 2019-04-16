using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace DotNetEtl.SqlServer
{
	public class DatabaseWriterCommandParameterProvider : IDatabaseWriterCommandParameterProvider
	{
		private IEnumerable<SqlParameter> staticParameters;

		public DatabaseWriterCommandParameterProvider(IEnumerable<SqlParameter> staticParameters)
		{
			this.staticParameters = staticParameters;
		}

		public DatabaseWriterCommandParameterProvider()
		{
		}

		public virtual IEnumerable<SqlParameter> GetParameters(object record)
		{
			var parameters = new List<SqlParameter>();

			if (this.staticParameters != null)
			{
				parameters.AddRange(staticParameters);
			}

			var properties = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				var parameterValue = property.GetValue(record);
				var commandParameterAttribute = property.GetCustomAttribute<SqlParameterAttribute>(true);

				if (commandParameterAttribute != null)
				{
					var parameter = new SqlParameter(commandParameterAttribute.Name, parameterValue);

					if (commandParameterAttribute.SqlDbType.HasValue)
					{
						parameter.SqlDbType = commandParameterAttribute.SqlDbType.Value;
					}

					if (commandParameterAttribute.Size.HasValue)
					{
						parameter.Size = commandParameterAttribute.Size.Value;
					}

					if (commandParameterAttribute.Precision.HasValue)
					{
						parameter.Precision = commandParameterAttribute.Precision.Value;
					}

					if (commandParameterAttribute.Scale.HasValue)
					{
						parameter.Scale = commandParameterAttribute.Scale.Value;
					}

					parameters.Add(parameter);
				}
			}

			return parameters;
		}
	}
}
