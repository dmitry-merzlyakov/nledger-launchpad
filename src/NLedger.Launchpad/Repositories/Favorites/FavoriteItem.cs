using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Favorites
{
    public sealed class FavoriteItem : IFavoriteItem
    {
        public FavoriteItem(Guid key, string title, Guid fileKey, string command)
        {
            if (key == default(Guid))
                throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrWhiteSpace(title))
                throw new ArgumentNullException(nameof(title));
            if (String.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));

            Key = key;
            Title = title;
            FileKey = fileKey;
            Command = command;
        }

        public Guid Key { get; }
        public string Title { get; }
        public Guid FileKey { get; }
        public string Command { get; }
    }
}
