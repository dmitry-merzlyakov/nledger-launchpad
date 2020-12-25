using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Logging
{
    public class AppLogEvents
    {
        public static EventId OnAfterRenderAsync = new EventId(2001, "OnAfterRenderAsync");
    }
}
