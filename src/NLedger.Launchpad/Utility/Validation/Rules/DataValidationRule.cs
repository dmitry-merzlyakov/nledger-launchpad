using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class DataValidationRule<T, D> : BaseDirectValidationRule<T>
    {
        public DataValidationRule(string message, Func<T, D> getData, Func<D, bool> checkData, string contextKey = null, int errorID = 0) 
            : base(message, contextKey, errorID)
        {
            if (getData == null)
                throw new ArgumentNullException(nameof(getData));
            if (checkData == null)
                throw new ArgumentNullException(nameof(checkData));

            GetData = getData;
            CheckData = checkData;
        }

        public Func<T,D> GetData { get; }
        public Func<D,bool> CheckData { get; }

        public override IEnumerable<ValidationRuleResult> Validate(T data, string outerContext = null)
        {
            if (data == null)
                yield return ValidationRuleResult.PositiveResult;
            else
                yield return CheckData(GetData(data)) ? ValidationRuleResult.PositiveResult : GetNegativeResult(outerContext);
        }
    }
}
