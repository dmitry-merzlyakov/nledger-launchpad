using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation
{
    public interface IValidator
    {
        ValidationResult ValidationResult { get; }
    }

    public class Validator<T> : IValidator
    {
        public Validator(ValidationRule<T> rootRule)
        {
            if (rootRule == null)
                throw new ArgumentNullException(nameof(rootRule));

            RootRule = rootRule;
        }

        public ValidationRule<T> RootRule { get; }
        public ValidationResult ValidationResult { get; private set; } = ValidationResult.Empty;

        public bool Validate(T data)
        {
            var results = RootRule.Validate(data).Where(r => !r.IsPositive);
            ValidationResult = results.Any() ? new ValidationResult(results) : ValidationResult.Empty;

            return ValidationResult.IsValid;
        }
    }
}
