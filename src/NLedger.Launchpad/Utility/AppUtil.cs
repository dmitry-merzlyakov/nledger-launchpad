using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility
{
    public static class AppUtil
    {
        public async static Task ReloadApplication(this IJSRuntime js)
        {
            if (js == null)
                throw new ArgumentNullException(nameof(js));

            await js.InvokeAsync<object>("reloadApp");
        }

    }
}
