using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public abstract class BaseDirectValidationRule<T> : ValidationRule<T>
    {
        public BaseDirectValidationRule(string message, string contextKey = null, int errorID = default(int))
        {
            if (String.IsNullOrEmpty(message))
                throw new ArgumentNullException(nameof(message));

            Message = message;
            ContextKey = contextKey;
            ErrorID = errorID;
        }

        public string Message { get; }
        public string ContextKey { get; }
        public int ErrorID { get; }

        protected ValidationRuleResult GetNegativeResult(string outerContext)
        {
            return new ValidationRuleResult(Message, String.IsNullOrEmpty(outerContext) ? ContextKey : $"{outerContext}:{ContextKey}", ErrorID);
        }
    }
}
