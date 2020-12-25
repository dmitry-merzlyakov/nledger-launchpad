using Microsoft.Extensions.Logging;
using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Config
{
    public class ConfigRepository : IConfigRepository
    {
        public static readonly string LocalStorageCommandHistoryList = "NLedger.Launchpad.CommandHistoryList";
        public static readonly string LocalStorageEnvironmentalConfig = "NLedger.Launchpad.EnvironmentalConfig";
        public static readonly string LocalStorageViewConfig = "NLedger.Launchpad.ViewConfig";

        public ConfigRepository(ILocalStorageRepository localStorageRepository)
        {
            if (localStorageRepository == null)
                throw new ArgumentNullException(nameof(localStorageRepository));

            LocalStorageRepository = localStorageRepository;
        }

        public ILocalStorageRepository LocalStorageRepository { get; }
        public ILogger<ConfigRepository> Logger { get; } = Logging.AppLoggingFactory.GetLogger<ConfigRepository>();


        public async Task<CommandHistoryList> GetCommandHistoryList()
        {
            return new ConfigSerializer().DeserializeCommandHistoryList(await LocalStorageRepository.GetItem(LocalStorageCommandHistoryList));
        }

        public async Task<EnvironmentalConfig> GetEnvironmentalConfig()
        {
            return new ConfigSerializer().DeserializeEnvironmentalConfig(await LocalStorageRepository.GetItem(LocalStorageEnvironmentalConfig));
        }

        public async Task<ViewConfig> GetViewConfig()
        {
            return new ConfigSerializer().DeserializeViewConfig(await LocalStorageRepository.GetItem(LocalStorageViewConfig));
        }

        public async Task SetCommandHistoryList(CommandHistoryList commandHistoryList)
        {
            await LocalStorageRepository.SetItem(LocalStorageCommandHistoryList, new ConfigSerializer().SerializeCommandHistoryList(commandHistoryList));
        }

        public async Task SetEnvironmentalConfig(EnvironmentalConfig environmentalConfig)
        {
            await LocalStorageRepository.SetItem(LocalStorageEnvironmentalConfig, new ConfigSerializer().SerializeEnvironmentalConfig(environmentalConfig));
        }

        public async Task SetViewConfig(ViewConfig viewConfig)
        {
            await LocalStorageRepository.SetItem(LocalStorageViewConfig, new ConfigSerializer().SerializeViewConfig(viewConfig));
        }
    }
}
