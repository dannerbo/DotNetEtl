using System;
using System.Reflection;

namespace DotNetEtl.Mapping
{
	public class FixedWidthBinaryRecordMapper : RecordMapper
	{
		public FixedWidthBinaryRecordMapper(
			IRecordFactory recordFactory,
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceFieldLayoutProvider sourceFieldLayoutProvider = null)
			: base(recordFactory, fieldParser ?? new BinaryFieldParser(), fieldDisplayNameProvider, fieldTransformer)
		{
			this.SourceFieldLayoutProvider = sourceFieldLayoutProvider ?? new SourceFieldLayoutProvider();
		}

		protected ISourceFieldLayoutProvider SourceFieldLayoutProvider { get; private set; }

		protected override bool TryReadSourceField(PropertyInfo property, object parsedSourceRecord, out object fieldValue, out string failureMessage)
		{
			fieldValue = null;
			failureMessage = null;
			
			if (this.TryGetSourceFieldLayout(property, parsedSourceRecord, out int startIndex, out int length))
			{
				try
				{
					fieldValue = new byte[length];

					Array.Copy((byte[])parsedSourceRecord, startIndex, (byte[])fieldValue, 0, length);
					
					return true;
				}
				catch (ArgumentOutOfRangeException)
				{
					failureMessage = "Field is missing or invalid.";

					return false;
				}
			}

			return false;
		}

		protected virtual bool TryGetSourceFieldLayout(PropertyInfo property, object record, out int startIndex, out int length)
		{
			return this.SourceFieldLayoutProvider.TryGetSourceFieldLayout(property, record, out startIndex, out length);
		}
	}

	public class FixedWidthBinaryRecordMapper<TRecord> : FixedWidthBinaryRecordMapper
		where TRecord : class, new()
	{
		public FixedWidthBinaryRecordMapper(
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceFieldLayoutProvider sourceFieldLayoutProvider = null)
			: base(new RecordFactory<TRecord>(), fieldParser, fieldDisplayNameProvider, fieldTransformer, sourceFieldLayoutProvider)
		{
		}
	}
}
