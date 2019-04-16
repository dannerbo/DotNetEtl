using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DotNetEtl.Mapping
{
	public class DelimitedTextRecordMapper : RecordMapper
	{
		public DelimitedTextRecordMapper(
			IRecordFactory recordFactory,
			string delimiter = ",",
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceRecordFieldCountProvider sourceRecordFieldCountProvider = null,
			ISourceFieldOrdinalProvider sourceFieldOrdinalProvider = null)
			: base(recordFactory, fieldParser, fieldDisplayNameProvider, fieldTransformer)
		{
			this.Delimiter = delimiter;
			this.SourceRecordFieldCountProvider = sourceRecordFieldCountProvider ?? new SourceRecordFieldCountProvider();
			this.SourceFieldOrdinalProvider = sourceFieldOrdinalProvider ?? new SourceFieldOrdinalProvider();
		}

		public string Delimiter { get; set; }
		protected ISourceRecordFieldCountProvider SourceRecordFieldCountProvider { get; private set; }
		protected ISourceFieldOrdinalProvider SourceFieldOrdinalProvider { get; private set; }

		protected override bool TryParseSourceRecord(object source, Type recordType, out object parsedSourceRecord, out string failureMessage)
		{
			if (String.IsNullOrEmpty(this.Delimiter))
			{
				throw new InvalidOperationException("Delimiter cannot be null or empty.");
			}

			parsedSourceRecord = null;
			failureMessage = null;

			var fieldValues = new List<string>();
			var pattern = String.Format("((?<=\")[^\"]*(?=\"({0}|$)+)|(?<={0}|^)[^{0}\"]*(?={0}|$))", Regex.Escape(this.Delimiter));

			foreach (Match match in Regex.Matches((string)source, pattern))
			{
				fieldValues.Add(match.Value);
			}

			if (this.TryGetSourceFieldCount(recordType, source, out int expectedFieldCount)
				&& fieldValues.Count != expectedFieldCount)
			{
				failureMessage = $"Expected {expectedFieldCount} fields but encountered {fieldValues.Count}.";

				return false;
			}

			parsedSourceRecord = fieldValues.ToArray();

			return true;
		}

		protected override bool TryReadSourceField(PropertyInfo property, object parsedSourceRecord, out object fieldValue, out string failureMessage)
		{
			fieldValue = null;
			failureMessage = null;

			if (this.TryGetSourceFieldOrdinal(property, parsedSourceRecord, out int ordinal))
			{
				if (((string[])parsedSourceRecord).Length - 1 < ordinal)
				{
					failureMessage = "Field is missing.";

					return false;
				}

				var parsedFieldValue = ((string[])parsedSourceRecord)[ordinal];

				if (parsedFieldValue.Length > 0)
				{
					fieldValue = parsedFieldValue;
				}

				return true;
			}

			return false;
		}

		protected virtual bool TryGetSourceFieldCount(Type recordType, object record, out int count)
		{
			return this.SourceRecordFieldCountProvider.TryGetSourceRecordFieldCount(recordType, record, out count);
		}

		protected virtual bool TryGetSourceFieldOrdinal(PropertyInfo property, object record, out int ordinal)
		{
			return this.SourceFieldOrdinalProvider.TryGetSourceFieldOrdinal(property, record, out ordinal);
		}
	}

	public class DelimitedTextRecordMapper<TRecord> : DelimitedTextRecordMapper
		where TRecord : class, new()
	{
		public DelimitedTextRecordMapper(
			string delimiter = ",",
			IFieldParser fieldParser = null,
			IFieldDisplayNameProvider fieldDisplayNameProvider = null,
			IFieldTransformer fieldTransformer = null,
			ISourceRecordFieldCountProvider sourceRecordFieldCountProvider = null,
			ISourceFieldOrdinalProvider sourceFieldOrdinalProvider = null)
			: base(new RecordFactory<TRecord>(), delimiter, fieldParser, fieldDisplayNameProvider, fieldTransformer, sourceRecordFieldCountProvider, sourceFieldOrdinalProvider)
		{
		}
	}
}
