using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Abstracts
{
    public interface ILocalStorageRepository
    {
        ValueTask<string> GetItem(string key);
        ValueTask SetItem(string key, string content);
        ValueTask RemoveItem(string key);
        Task<IEnumerable<string>> GetKeys();
    }
}
