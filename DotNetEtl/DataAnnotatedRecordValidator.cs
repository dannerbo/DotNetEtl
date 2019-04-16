using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DotNetEtl
{
	public class DataAnnotatedRecordValidator : IRecordValidator
	{
		public DataAnnotatedRecordValidator(IFieldDisplayNameProvider fieldDisplayNameProvider = null)
		{
			this.FieldDisplayNameProvider = fieldDisplayNameProvider;
		}

		public DataAnnotatedRecordValidator()
			: this(new FieldDisplayNameProvider())
		{
		}

		protected IFieldDisplayNameProvider FieldDisplayNameProvider { get; private set; }

		public virtual bool TryValidate(object record, out IEnumerable<FieldFailure> failures)
		{
			failures = new List<FieldFailure>();

			var failuresList = (List<FieldFailure>)failures;
			var validationContext = new ValidationContext(record);
			var validationResults = new List<ValidationResult>();

			if (!Validator.TryValidateObject(record, validationContext, validationResults, true))
			{
				foreach (var validationResult in validationResults)
				{
					var fieldName = validationResult.MemberNames.Single();
					var property = record.GetType().GetProperty(fieldName);
					var fieldValue = property.GetValue(record);
					var fieldDisplayName = this.FieldDisplayNameProvider?.GetFieldDisplayName(property) ?? fieldName;

					failuresList.Add(new FieldFailure()
					{
						FieldName = fieldDisplayName,
						FieldValue = fieldValue,
						Message = validationResult.ErrorMessage
					});
				}
			}

			return failuresList.Count == 0;
		}
	}
}
