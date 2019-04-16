using System;
using System.Data;
using System.Reflection;

namespace DotNetEtl.Mapping
{
	public class DataRecordRecordMapper : RecordMapper
	{
		public DataRecordRecordMapper(
			IRecordFactory recordFactory,
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceFieldOrdinalProvider sourceFieldOrdinalProvider = null,
			ISourceFieldNameProvider sourceFieldNameProvider = null)
			: base(recordFactory, fieldParser, fieldDisplayNameProvider, fieldTransformer)
		{
			this.SourceFieldOrdinalProvider = sourceFieldOrdinalProvider ?? new SourceFieldOrdinalProvider();
			this.SourceFieldNameProvider = sourceFieldNameProvider ?? new SourceFieldNameProvider();
		}

		protected ISourceFieldOrdinalProvider SourceFieldOrdinalProvider { get; private set; }
		protected ISourceFieldNameProvider SourceFieldNameProvider { get; private set; }

		protected override bool TryReadSourceField(PropertyInfo property, object parsedSourceRecord, out object fieldValue, out string failureMessage)
		{
			fieldValue = null;
			failureMessage = null;

			if (!this.TryGetSourceFieldOrdinal(property, parsedSourceRecord, out int ordinal))
			{
				if (this.TryGetSourceFieldName(property, parsedSourceRecord, out string fieldName))
				{
					try
					{
						ordinal = ((IDataRecord)parsedSourceRecord).GetOrdinal(fieldName);
					}
					catch (IndexOutOfRangeException)
					{
						failureMessage = "Field is missing or invalid.";

						return false;
					}
				}
				else
				{
					return false;
				}
			}

			try
			{
				var dataRecord = (IDataRecord)parsedSourceRecord;

				fieldValue = !dataRecord.IsDBNull(ordinal)
					? dataRecord.GetValue(ordinal)
					: null;

				return true;
			}
			catch (IndexOutOfRangeException)
			{
				failureMessage = "Field is missing or invalid.";

				return false;
			}
		}

		private bool TryGetSourceFieldName(PropertyInfo property, object record, out string fieldName)
		{
			return this.SourceFieldNameProvider.TryGetSourceFieldName(property, record, out fieldName);
		}

		protected virtual bool TryGetSourceFieldOrdinal(PropertyInfo property, object record, out int ordinal)
		{
			return this.SourceFieldOrdinalProvider.TryGetSourceFieldOrdinal(property, record, out ordinal);
		}
	}

	public class DataRecordRecordMapper<TRecord> : DataRecordRecordMapper
		where TRecord : class, new()
	{
		public DataRecordRecordMapper(
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceFieldOrdinalProvider sourceFieldOrdinalProvider = null,
			ISourceFieldNameProvider sourceFieldNameProvider = null)
			: base(new RecordFactory<TRecord>(), fieldParser, fieldDisplayNameProvider, fieldTransformer, sourceFieldOrdinalProvider, sourceFieldNameProvider)
		{
		}
	}
}
