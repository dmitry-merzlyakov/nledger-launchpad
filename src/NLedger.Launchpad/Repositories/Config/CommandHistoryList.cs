using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Config
{
    public class CommandHistoryList
    {
        public CommandHistoryList()
        { }

        public CommandHistoryList(IEnumerable<string> commands)
        {
            Commands = new List<string>(commands ?? Enumerable.Empty<string>());
        }

        public IList<string> Commands { get; } = new List<string>();

        public bool AddCommand(string command)
        {
            if (String.IsNullOrWhiteSpace(command))
                return false;

            var index = Commands.IndexOf(command);
            if (index >= 0)
            {
                if (index == Commands.Count - 1)
                    return false;

                Commands.RemoveAt(index);
            }

            Commands.Add(command);
            return true;
        }
    }
}
