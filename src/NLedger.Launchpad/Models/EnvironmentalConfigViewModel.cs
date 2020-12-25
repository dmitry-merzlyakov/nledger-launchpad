using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class EnvironmentVariableModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class EnvironmentalConfigViewModel
    {
        public bool IsAtty { get; set; }
        public string TimeZoneId { get; set; }
        public int OutputEncoding { get; set; }
        public List<EnvironmentVariableModel> EnvironmentVariables { get; set; }


        public static TimeZoneInfo[] TimeZones { get; private set; }
        public static EncodingInfo[] Encodings { get; private set; }
        public static bool IsInitialized { get; private set; }
        public static void EnsureInitialized()
        {
            if (!IsInitialized)
            {
                IsInitialized = true;
                Encodings = Encoding.GetEncodings();
                TimeZones = TimeZoneInfo.GetSystemTimeZones().ToArray();
            }
        }

    }
}
