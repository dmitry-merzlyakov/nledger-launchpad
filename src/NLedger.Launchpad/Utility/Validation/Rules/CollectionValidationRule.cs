using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class CollectionValidationRule<T, P> : ValidationRule<T>
    {
        public CollectionValidationRule(Func<T, IEnumerable<P>> getCollection, ValidationRule<P> rule)
        {
            if (getCollection == null)
                throw new ArgumentNullException(nameof(getCollection));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            GetCollection = getCollection;
            Rule = rule;
        }

        public Func<T, IEnumerable<P>> GetCollection { get; }
        public ValidationRule<P> Rule { get; }

        public override IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null)
        {
            return (GetCollection(data) ?? Enumerable.Empty<P>()).GetIndexed().
                SelectMany(p => Rule.Validate(p.Value, p.GetContextKey(outerContext)));
        }

    }
}
