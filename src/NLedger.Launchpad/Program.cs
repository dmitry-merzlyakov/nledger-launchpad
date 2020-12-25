using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLedger.Launchpad.Services;
using Blazored.LocalStorage;
using NLedger.Launchpad.Repositories;
using NLedger.Launchpad.Logging;

namespace NLedger.Launchpad
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddSingleton(sp => new DialogService());
            builder.Services.AddSingleton(sp => new AlertService());
            builder.Services.AddSingleton(typeof(ModelService));

            builder.Logging.SetMinimumLevel(LogLevel.Error);

            var host = builder.Build();
            AppLoggingFactory.Init(host.Services.GetRequiredService<ILoggerFactory>());

            await host.RunAsync();
        }
    }
}
