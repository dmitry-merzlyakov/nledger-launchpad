using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation
{
    public class ValidationResult
    {
        public static readonly ValidationResult Empty = new ValidationResult(null);

        public ValidationResult(IEnumerable<ValidationRuleResult> ruleResults)
        {
            RuleResults = (ruleResults ?? Enumerable.Empty<ValidationRuleResult>()).Where(r => !r.IsPositive).ToArray();
        }

        public IEnumerable<ValidationRuleResult> RuleResults { get; }

        public bool IsValid => !RuleResults.Any();

        public string GetMessages(string contextKey = null)
        {
            var sb = new StringBuilder();
            foreach (var result in RuleResults.Where(r => r.ContextKey == contextKey))
                sb.AppendLine(result.Message);
            return sb.ToString();
        }
    }
}
