using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility
{
    public static class FileUtil
    {
        /// <summary>
        /// Based on a solution described here: 
        /// https://stackoverflow.com/questions/52683706/how-can-one-generate-and-save-a-file-client-side-using-blazor
        /// </summary>
        public async static Task SaveAs(this IJSRuntime js, string filename, byte[] data)
        {
            if (js == null)
                throw new ArgumentNullException(nameof(js));
            if (String.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException(nameof(filename));
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            await js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
        }
    }
}
