using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class CompositeValidationRule<T> : ValidationRule<T>
    {
        public CompositeValidationRule(IEnumerable<ValidationRule<T>> rules)
        {
            Rules = (rules ?? Enumerable.Empty<ValidationRule<T>>()).ToArray();
        }

        public IEnumerable<ValidationRule<T>> Rules { get; }

        public override IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null)
        {
            foreach (var rule in Rules)
                foreach (var ruleResult in rule.Validate(data, outerContext))
                    yield return ruleResult;
        }
    }
}
