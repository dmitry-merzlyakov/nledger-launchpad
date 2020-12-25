using NLedger.Launchpad.Utility.Validation.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility.Validation
{
    // Validator's builder context; it is only a generalization constraint. No instances needed.
    public sealed class ValidatorBuilderContext<T>
    {
        public static ValidatorBuilderContext<T> Context => null;
    }

    public static class Validators
    {
        public static ValidationRule<string> GetFileSystemNameValidationRule()
        {
            return new CompositeValidationRule<string>(new ValidationRule<string>[]
            {
                new CustomValidationRule<string>(s => !String.IsNullOrWhiteSpace(s), "Please fill out the name. It should not be empty."),
                new CustomValidationRule<string>(s => s != "." && s != "..", "Please provide a valid name ('.' and '..' are reserved)"),
                new CustomValidationRule<string>(s => (s ?? String.Empty).IndexOfAny(DirectorySeparators) < 0, "Please provide a valid name (it should not contain a directory separator)")
            });
        }

        public static Validator<string> GetFileSystemNameValidator()
        {
            return new Validator<string>(GetFileSystemNameValidationRule());
        }

        // Validator builder

        public static Validator<T> BuildFor<T>(Func<ValidatorBuilderContext<T>,ValidationRule<T>> creator)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            return new Validator<T>(creator(ValidatorBuilderContext<T>.Context));
        }

        public static ValidationRule<T> Custom<T>(this ValidatorBuilderContext<T> context, Func<T, bool> checkData, string message, string contextKey = null)
        {
            return new CustomValidationRule<T>(checkData, message, contextKey);
        }

        public static ValidationRule<T> Composite<T>(this ValidatorBuilderContext<T> context, params Func<ValidatorBuilderContext<T>, ValidationRule<T>>[] creators)
        {
            var rules = (creators ?? Enumerable.Empty<Func<ValidatorBuilderContext<T>, ValidationRule<T>>>()).Select(c => c(ValidatorBuilderContext<T>.Context));
            return new CompositeValidationRule<T>(rules.ToArray());
        }

        public static ValidationRule<T> Collection<T,P>(this ValidatorBuilderContext<T> context, Func<T, IEnumerable<P>> getCollection, Func<ValidatorBuilderContext<P>, ValidationRule<P>> creator)
        {
            if (creator == null)
                throw new ArgumentNullException(nameof(creator));

            return new CollectionValidationRule<T,P>(getCollection, creator(ValidatorBuilderContext<P>.Context));
        }

        public static ValidationRule<T> Checkpoint<T>(this ValidatorBuilderContext<T> context, Func<ValidatorBuilderContext<T>, ValidationRule<T>> primary, Func<ValidatorBuilderContext<T>, ValidationRule<T>> secondary)
        {
            return new CheckpointRule<T>(primary(ValidatorBuilderContext<T>.Context), secondary(ValidatorBuilderContext<T>.Context));
        }

        public static ValidationRule<T> Executive<T>(this ValidatorBuilderContext<T> context, Func<ValidatorBuilderContext<T>, ValidationRule<T>> underlyingRule)
        {
            return new ExecutiveRule<T>(underlyingRule(ValidatorBuilderContext<T>.Context));
        }

        private static readonly char[] DirectorySeparators = new char[] { '/', '\\' };
    }
}
