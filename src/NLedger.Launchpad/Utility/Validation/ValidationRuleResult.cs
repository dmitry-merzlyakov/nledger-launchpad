using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation
{
    public sealed class ValidationRuleResult
    {
        public static readonly ValidationRuleResult PositiveResult = new ValidationRuleResult();

        public ValidationRuleResult(string message, string contextKey = null, int errorID = default(int))
        {
            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            Message = message;
            ContextKey = contextKey;
            ErrorID = errorID;
        }

        private ValidationRuleResult()
        { }

        public string Message { get; }
        public string ContextKey { get; }
        public int ErrorID { get; }

        public bool IsPositive => Message == null;
    }
}
