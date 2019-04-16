
using System.Reflection;

namespace DotNetEtl.Mapping
{
	public class ObjectRecordMapper : RecordMapper
	{
		public ObjectRecordMapper(
			IRecordFactory recordFactory,
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceFieldNameProvider sourceFieldNameProvider = null)
			: base(recordFactory, fieldParser, fieldDisplayNameProvider, fieldTransformer)
		{
			this.SourceFieldNameProvider = sourceFieldNameProvider ?? new SourceFieldNameProvider();
		}

		protected ISourceFieldNameProvider SourceFieldNameProvider { get; private set; }

		protected override bool TryReadSourceField(PropertyInfo property, object parsedSourceRecord, out object fieldValue, out string failureMessage)
		{
			fieldValue = null;
			failureMessage = null;
			
			if (this.TryGetSourceFieldName(property, parsedSourceRecord, out string fieldName))
			{
				var sourceProperty = parsedSourceRecord.GetType().GetProperty(fieldName);

				if (sourceProperty == null)
				{
					failureMessage = "Field is missing.";

					return false;
				}

				fieldValue = sourceProperty.GetValue(parsedSourceRecord);

				return true;
			}

			return false;
		}

		protected virtual bool TryGetSourceFieldName(PropertyInfo property, object record, out string fieldName)
		{
			return this.SourceFieldNameProvider.TryGetSourceFieldName(property, record, out fieldName);
		}
	}

	public class ObjectRecordMapper<TRecord> : ObjectRecordMapper
		where TRecord : class, new()
	{
		public ObjectRecordMapper(
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceFieldNameProvider sourceFieldNameProvider = null)
			: base(new RecordFactory<TRecord>(), fieldParser, fieldDisplayNameProvider, fieldTransformer, sourceFieldNameProvider)
		{
		}
	}
}
