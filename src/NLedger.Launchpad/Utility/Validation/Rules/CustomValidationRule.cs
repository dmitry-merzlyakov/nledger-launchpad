using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class CustomValidationRule<T> : BaseDirectValidationRule<T>
    {
        public CustomValidationRule(Func<T, bool> checkData, string message, string contextKey = null, int errorID = 0) 
            : base(message, contextKey, errorID)
        {
            if (checkData == null)
                throw new ArgumentNullException(nameof(checkData));

            CheckData = checkData;
        }

        public Func<T,bool> CheckData { get; }

        public override IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null)
        {
            yield return CheckData(data) ? ValidationRuleResult.PositiveResult : GetNegativeResult(outerContext);
        }
    }
}
