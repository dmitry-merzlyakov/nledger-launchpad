using NLedger.Launchpad.Repositories.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Abstracts
{
    public interface IConfigRepository
    {
        Task<EnvironmentalConfig> GetEnvironmentalConfig();
        Task SetEnvironmentalConfig(EnvironmentalConfig environmentalConfig);

        Task<ViewConfig> GetViewConfig();
        Task SetViewConfig(ViewConfig viewConfig);

        Task<CommandHistoryList> GetCommandHistoryList();
        Task SetCommandHistoryList(CommandHistoryList commandHistoryList);
    }
}
