using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class CheckpointRule<T> : ValidationRule<T>
    {
        public CheckpointRule(ValidationRule<T> primaryRule, ValidationRule<T> secondaryRule)
        {
            if (primaryRule == null)
                throw new ArgumentNullException(nameof(primaryRule));
            if (secondaryRule == null)
                throw new ArgumentNullException(nameof(secondaryRule));

            PrimaryRule = primaryRule;
            SecondaryRule = secondaryRule;
        }

        public ValidationRule<T> PrimaryRule { get; }
        public ValidationRule<T> SecondaryRule { get; }

        public override IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null)
        {
            var results = PrimaryRule.Validate(data, outerContext).ToList();

            if (results.All(p => p.IsPositive))
                results.AddRange(SecondaryRule.Validate(data, outerContext).ToArray());

            return results;
        }

    }
}
