using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Config
{
    public class ConfigSerializer
    {
        public class CommandHistoryListDto
        {
            public List<string> Commands { get; set; }
        }

        public class EnvironmentVariableDto
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class EnvironmentalConfigDto
        {
            public bool IsAtty { get; set; }
            public string TimeZoneId { get; set; }
            public string OutputEncoding { get; set; }
            public List<EnvironmentVariableDto> EnvironmentVariables { get; set; }
        }


        public CommandHistoryList DeserializeCommandHistoryList(string s)
        {
            if (!String.IsNullOrWhiteSpace(s))
                return new CommandHistoryList(System.Text.Json.JsonSerializer.Deserialize<CommandHistoryListDto>(s).Commands);
            else
                return new CommandHistoryList();
        }

        public string SerializeCommandHistoryList(CommandHistoryList commandHistoryList)
        {
            if (commandHistoryList == null)
                throw new ArgumentNullException(nameof(commandHistoryList));

            return System.Text.Json.JsonSerializer.Serialize(new CommandHistoryListDto() { Commands = commandHistoryList.Commands.ToList() });
        }

        public EnvironmentalConfig DeserializeEnvironmentalConfig(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return new EnvironmentalConfig();

            var dto = System.Text.Json.JsonSerializer.Deserialize<EnvironmentalConfigDto>(s);

            return new EnvironmentalConfig(dto.IsAtty, 
                TimeZoneInfo.FindSystemTimeZoneById(dto.TimeZoneId),
                Encoding.GetEncoding(dto.OutputEncoding), 
                (dto.EnvironmentVariables ?? Enumerable.Empty<EnvironmentVariableDto>()).ToDictionary(t => t.Name, t => t.Value));
        }

        public string SerializeEnvironmentalConfig(EnvironmentalConfig environmentalConfig)
        {
            if (environmentalConfig == null)
                throw new ArgumentNullException(nameof(environmentalConfig));

            var dto = new EnvironmentalConfigDto()
            {
                IsAtty = environmentalConfig.IsAtty,
                TimeZoneId = environmentalConfig.TimeZone.Id,
                OutputEncoding = environmentalConfig.OutputEncoding.WebName,
                EnvironmentVariables = environmentalConfig.EnvironmentVariables.Select(kv => new EnvironmentVariableDto() { Name = kv.Key, Value = kv.Value }).ToList()
            };

            return System.Text.Json.JsonSerializer.Serialize(dto);
        }

        public ViewConfig DeserializeViewConfig(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return new ViewConfig();

            return System.Text.Json.JsonSerializer.Deserialize<ViewConfig>(s);
        }

        public string SerializeViewConfig(ViewConfig viewConfig)
        {
            if (viewConfig == null)
                throw new ArgumentNullException(nameof(viewConfig));

            return System.Text.Json.JsonSerializer.Serialize(viewConfig);
        }
    }
}
