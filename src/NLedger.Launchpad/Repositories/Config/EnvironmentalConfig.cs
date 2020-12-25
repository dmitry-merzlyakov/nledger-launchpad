using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Config
{
    public class EnvironmentalConfig
    {
        public EnvironmentalConfig()
        { }

        public EnvironmentalConfig(bool isAtty, TimeZoneInfo timeZone, Encoding outputEncoding, IDictionary<string,string> environmentVariables)
        {
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));
            if (outputEncoding == null)
                throw new ArgumentNullException(nameof(outputEncoding));

            IsAtty = isAtty;
            TimeZone = timeZone;
            OutputEncoding = outputEncoding;

            if (environmentVariables != null)
                EnvironmentVariables = environmentVariables;
        }

        public bool IsAtty { get; } = true;
        public TimeZoneInfo TimeZone { get; } = TimeZoneInfo.Local;
        public Encoding OutputEncoding { get; } = Encoding.UTF8;
        public IDictionary<string, string> EnvironmentVariables { get; } = new Dictionary<string, string>();
    }
}
