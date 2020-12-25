using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class ExecutiveRule<T> : ValidationRule<T>
    {
        public ExecutiveRule(ValidationRule<T> underlyingRule)
        {
            if (underlyingRule == null)
                throw new ArgumentNullException(nameof(underlyingRule));

            UnderlyingRule = underlyingRule;
        }

        public ValidationRule<T> UnderlyingRule { get; }

        public override IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null)
        {
            try
            {
                return UnderlyingRule.Validate(data, outerContext);
            }
            catch(Exception ex)
            {
                return new ValidationRuleResult[] { new ValidationRuleResult(ex.Message, outerContext) };
            }
        }
    }
}
