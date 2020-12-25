using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation
{
    public abstract class ValidationRule<T>
    {
        public abstract IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null);
    }
}
