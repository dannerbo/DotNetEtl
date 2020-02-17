using System;
using System.Collections.Generic;
using System.Reflection;

namespace DotNetEtl
{
	public abstract class RecordMapper : IRecordMapper
	{
		public RecordMapper(IRecordFactory recordFactory, IFieldParser fieldParser = null, IFieldDisplayNameProvider fieldDisplayNameProvider = null, IFieldTransformer fieldTransformer = null)
		{
			this.RecordFactory = recordFactory;
			this.FieldParser = fieldParser ?? new FieldParser();
			this.FieldDisplayNameProvider = fieldDisplayNameProvider ?? new FieldDisplayNameProvider();
			this.FieldTransformer = fieldTransformer ?? new FieldTransformer();
		}

		protected IRecordFactory RecordFactory { get; private set; }
		protected IFieldParser FieldParser { get; private set; }
		protected IFieldDisplayNameProvider FieldDisplayNameProvider { get; private set; }
		protected IFieldTransformer FieldTransformer { get; private set; }

		protected abstract bool TryReadSourceField(PropertyInfo property, object parsedSourceRecord, out object fieldValue, out string failureMessage);

		public virtual bool TryMap(object source, out object record, out IEnumerable<FieldFailure> failures)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			record = this.RecordFactory.Create(source);

			if (!this.TryParseSourceRecord(source, record.GetType(), out object parsedSourceRecord, out string failureMessage))
			{
				failures = new List<FieldFailure>();
				
				((List<FieldFailure>)failures).Add(new FieldFailure() { Message = failureMessage });

				return false;
			}

			return this.TrySetRecordPropertyValues(record, parsedSourceRecord, out failures);
		}

		protected virtual bool TryParseSourceRecord(object source, Type recordType, out object parsedSourceRecord, out string failureMessage)
		{
			parsedSourceRecord = source;
			failureMessage = null;

			return true;
		}

		protected virtual bool TrySetRecordPropertyValues(object record, object parsedSourceRecord, out IEnumerable<FieldFailure> failures)
		{
			failures = new List<FieldFailure>();
			var failuresList = (List<FieldFailure>)failures;

			var properties = record.GetType().GetCachedProperties();

			foreach (var property in properties)
			{
				if (this.TryReadSourceField(property, parsedSourceRecord, out object fieldValue, out string failureMessage)
					&& fieldValue != null)
				{
					if (this.TryParseSourceField(property, fieldValue, out object parsedFieldValue, out failureMessage))
					{
						property.SetValue(record, parsedFieldValue);
					}
					else if (failureMessage != null)
					{
						failuresList.Add(
							new FieldFailure()
							{
								Message = failureMessage,
								FieldName = this.FieldDisplayNameProvider.GetFieldDisplayName(property),
								FieldValue = fieldValue
							});

						continue;
					}
				}
				else if (failureMessage != null)
				{
					failuresList.Add(
						new FieldFailure()
						{
							Message = failureMessage,
							FieldName = this.FieldDisplayNameProvider.GetFieldDisplayName(property)
						});

					continue;
				}

				this.ApplyFieldTransforms(property, record);
			}

			return failuresList.Count == 0;
		}

		protected virtual bool TryParseSourceField(PropertyInfo property, object fieldValue, out object parsedFieldValue, out string failureMessage)
		{
			return this.FieldParser.TryParse(property, fieldValue, out parsedFieldValue, out failureMessage);
		}

		protected virtual void ApplyFieldTransforms(PropertyInfo property, object record)
		{
			this.FieldTransformer.ApplyTransforms(property, record);
		}
	}
}
