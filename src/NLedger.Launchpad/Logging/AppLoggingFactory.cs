using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Logging
{
    public class AppLoggingFactory
    {
        public static void Init(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));
            if (LoggerFactory != null)
                throw new InvalidOperationException("Alreay initialized");

            LoggerFactory = loggerFactory;
        }

        public static ILogger<T> GetLogger<T>()
        {
            if (LoggerFactory == null)
                throw new InvalidOperationException("Not initialized yet");

            return LoggerFactory.CreateLogger<T>();
        }

        private static ILoggerFactory LoggerFactory { get; set; }
    }
}
