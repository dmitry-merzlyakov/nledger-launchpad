using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories
{
    public class LocalStorageRepository : ILocalStorageRepository
    {
        public LocalStorageRepository(ISyncLocalStorageService localStorageService)
        {
            if (localStorageService == null)
                throw new ArgumentNullException(nameof(localStorageService));

            LocalStorageService = localStorageService;
        }

        public ISyncLocalStorageService LocalStorageService { get; set; }
        public ILogger<LocalStorageRepository> Logger { get; } = Logging.AppLoggingFactory.GetLogger<LocalStorageRepository>();

        public async ValueTask<string> GetItem(string key)
        {
            Logger.LogDebug("Getting item {key}", key);

            var content = String.Empty;
            if (LocalStorageService.ContainKey(key))
            {
                Logger.LogDebug("Item {key} exists in the local storage; loading", key);
                content = LocalStorageService.GetItemAsString(key);
            }

            return await Task.FromResult(content);
        }

        public async Task<IEnumerable<string>> GetKeys()
        {
            var length = LocalStorageService.Length();

            var keys = new List<string>();
            for (int i = 0; i < length; i++)
                keys.Add(LocalStorageService.Key(i));

            return await Task.FromResult(keys);
        }

        public async ValueTask RemoveItem(string key)
        {
            LocalStorageService.RemoveItem(key);
            await Task.Delay(1);
        }

        public async ValueTask SetItem(string key, string content)
        {
            LocalStorageService.SetItem(key, content);
            await Task.Delay(1);
        }
    }
}
