using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Abstracts
{
    public interface IFavoriteItem
    {
        Guid Key { get; }
        string Title { get; }
        Guid FileKey { get; }
        string Command { get; }
    }
}
