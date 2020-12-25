using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class ProjectionValidationRule<T, P> : ValidationRule<T>
    {
        public ProjectionValidationRule(Func<T, P> getProjection, ValidationRule<P> rule)
        {
            if (getProjection == null)
                throw new ArgumentNullException(nameof(getProjection));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            GetProjection = getProjection;
            Rule = rule;
        }

        public Func<T,P> GetProjection { get; }
        public ValidationRule<P> Rule { get; }

        public override IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null)
        {
            return Rule.Validate(GetProjection(data), outerContext);
        }
    }
}
