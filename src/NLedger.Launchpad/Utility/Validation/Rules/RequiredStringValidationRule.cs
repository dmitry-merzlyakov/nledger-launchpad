using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation.Rules
{
    public class RequiredStringValidationRule<T> : DataValidationRule<T, string>
    {
        public RequiredStringValidationRule(string message, Func<T, string> getData, string contextKey = null, int errorID = 0) 
            : base(message, getData, d => String.IsNullOrWhiteSpace(d), contextKey, errorID)
        {
        }
    }
}
