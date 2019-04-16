using System;
using System.Reflection;

namespace DotNetEtl.Mapping
{
	public class FixedWidthTextRecordMapper : RecordMapper
	{
		public FixedWidthTextRecordMapper(
			IRecordFactory recordFactory,
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceRecordLengthProvider sourceRecordLengthProvider = null,
			ISourceFieldLayoutProvider sourceFieldLayoutProvider = null)
			: base(recordFactory, fieldParser, fieldDisplayNameProvider, fieldTransformer)
		{
			this.SourceRecordLengthProvider = sourceRecordLengthProvider ?? new SourceRecordLengthProvider();
			this.SourceFieldLayoutProvider = sourceFieldLayoutProvider ?? new SourceFieldLayoutProvider();
		}

		public bool ShouldTrimFields { get; set; } = true;
		protected ISourceRecordLengthProvider SourceRecordLengthProvider { get; private set; }
		protected ISourceFieldLayoutProvider SourceFieldLayoutProvider { get; private set; }

		protected override bool TryParseSourceRecord(object source, Type recordType, out object parsedSourceRecord, out string failureMessage)
		{
			parsedSourceRecord = null;
			failureMessage = null;

			var recordLength = ((string)source).Length;

			if (this.TryGetSourceRecordLength(recordType, source, out int expectedRecordLength)
				&& recordLength != expectedRecordLength)
			{
				failureMessage = $"Expected record length of {expectedRecordLength} but encountered {recordLength}.";

				return false;
			}

			parsedSourceRecord = source;

			return true;
		}

		protected override bool TryReadSourceField(PropertyInfo property, object parsedSourceRecord, out object fieldValue, out string failureMessage)
		{
			fieldValue = null;
			failureMessage = null;

			if (this.TryGetSourceFieldLayout(property, parsedSourceRecord, out int startIndex, out int length))
			{
				try
				{
					var parsedFieldValue = ((string)parsedSourceRecord).Substring(startIndex, length);

					if (this.ShouldTrimFields || property.PropertyType != typeof(string))
					{
						parsedFieldValue = parsedFieldValue.Trim();
					}

					if (parsedFieldValue.Length > 0)
					{
						fieldValue = parsedFieldValue;
					}

					return true;
				}
				catch (ArgumentOutOfRangeException)
				{
					failureMessage = "Field is invalid.";

					return false;
				}
			}

			return false;
		}

		protected virtual bool TryGetSourceRecordLength(Type recordType, object record, out int length)
		{
			return this.SourceRecordLengthProvider.TryGetSourceRecordLength(recordType, record, out length);
		}

		protected virtual bool TryGetSourceFieldLayout(PropertyInfo property, object record, out int startIndex, out int length)
		{
			return this.SourceFieldLayoutProvider.TryGetSourceFieldLayout(property, record, out startIndex, out length);
		}
	}

	public class FixedWidthTextRecordMapper<TRecord> : FixedWidthTextRecordMapper
		where TRecord : class, new()
	{
		public FixedWidthTextRecordMapper(
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceRecordLengthProvider sourceRecordLengthProvider = null,
			ISourceFieldLayoutProvider sourceFieldLayoutProvider = null)
			: base(new RecordFactory<TRecord>(), fieldParser, fieldDisplayNameProvider, fieldTransformer, sourceRecordLengthProvider, sourceFieldLayoutProvider)
		{
		}
	}
}
