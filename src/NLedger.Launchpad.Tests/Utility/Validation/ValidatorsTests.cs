using NLedger.Launchpad.Utility.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NLedger.Launchpad.Tests.Utility.Validation
{
    public class ValidatorsTests
    {
        [Fact]
        public void Validators_GetFileSystemNameValidator_IntegrationTests()
        {
            var fileNameValidator = Validators.GetFileSystemNameValidator();

            Assert.False(fileNameValidator.Validate(null));
            Assert.Equal("Please fill out the name. It should not be empty.", fileNameValidator.ValidationResult.GetMessages().TrimEnd());

        }
    }
}
