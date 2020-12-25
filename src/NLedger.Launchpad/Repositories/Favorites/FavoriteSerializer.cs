using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Favorites
{
    public class FavoriteSerializer
    {
        public string Serialize(IEnumerable<FavoriteItem> favorites)
        {
            if (favorites == null)
                throw new ArgumentNullException(nameof(favorites));

            var items = favorites.Select(f => new FavoriteItemDto()
            {
                Key = f.Key,
                Title = f.Title,
                FileKey = f.FileKey,
                Command = f.Command
            }).ToList();

            return System.Text.Json.JsonSerializer.Serialize(new FavoriteItemDtoContainer() { Items = items });
        }

        public IEnumerable<FavoriteItem> Deserialize(string serializedItems)
        {
            if (String.IsNullOrWhiteSpace(serializedItems))
                return Enumerable.Empty<FavoriteItem>();

            var container = System.Text.Json.JsonSerializer.Deserialize<FavoriteItemDtoContainer>(serializedItems);
            var items = (container?.Items ?? Enumerable.Empty<FavoriteItemDto>()).Select(f => new FavoriteItem(f.Key, f.Title, f.FileKey, f.Command)).ToArray();
            return items;
        }

    }
}
