using Microsoft.Extensions.Logging;
using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLedger.Launchpad.Utility;

namespace NLedger.Launchpad.Repositories.Favorites
{
    public class FavoriteRepository : IFavoriteRepository
    {
        public static readonly string LocalStorageFavoritesName = "NLedger.Launchpad.Favorites";

        public FavoriteRepository(ILocalStorageRepository localStorageRepository)
        {
            if (localStorageRepository == null)
                throw new ArgumentNullException(nameof(localStorageRepository));

            LocalStorageRepository = localStorageRepository;
        }

        public ILocalStorageRepository LocalStorageRepository { get; }
        public ILogger<FavoriteRepository> Logger { get; } = Logging.AppLoggingFactory.GetLogger<FavoriteRepository>();



        public event EventHandler FavoritesChanged;

        public async Task<IFavoriteItem> CreateFavorite(string title, Guid fileKey, string command, Guid key = default)
        {
            var item = new FavoriteItem(key == default(Guid) ? Guid.NewGuid() : key, title, fileKey, command);
            if ((await GetItem(item.Key)) != null)
                throw new InvalidOperationException($"Favorite key {item.Key} already exists");

            return await ProcessChanges(item, (itm, items) => items.AddItem(itm));
        }

        public async Task<IFavoriteItem> DeleteFavorite(Guid key)
        {
            var item = (await GetItem(key)) as FavoriteItem;
            if (item == null)
                throw new InvalidOperationException($"Favorite item {key} not found.");

            return await ProcessChanges(item, (itm, items) => items.Where(f => f.Key != itm.Key));
        }

        public async Task<IFavoriteItem> EditFavorite(string title, Guid fileKey, string command, Guid key)
        {
            var item = (await GetItem(key)) as FavoriteItem;
            if (item == null)
                throw new InvalidOperationException($"Favorite item {key} not found.");

            var newItem = new FavoriteItem(key, title, fileKey, command);
            return await ProcessChanges(item, (itm, items) => items.Where(f => f.Key != itm.Key).AddItem(newItem));
        }

        public async Task<IEnumerable<IFavoriteItem>> GetFavorites()
        {
            if (Items == null)
                Items = new FavoriteSerializer().Deserialize(await LocalStorageRepository.GetItem(LocalStorageFavoritesName)).ToList();

            return Items.ToArray();
        }

        public async Task<IFavoriteItem> GetItem(Guid key)
        {
            return (await GetFavorites()).FirstOrDefault(f => f.Key == key);
        }

        private async Task<FavoriteItem> ProcessChanges(FavoriteItem item, Func<FavoriteItem, List<FavoriteItem>, IEnumerable<FavoriteItem>> action)
        {
            await GetFavorites();
            Items = action(item, Items).ToList();

            var serializedFavorites = new FavoriteSerializer().Serialize(Items);
            await LocalStorageRepository.SetItem(LocalStorageFavoritesName, serializedFavorites);

            FavoritesChanged?.Invoke(this, EventArgs.Empty);
            return item;
        }

        private List<FavoriteItem> Items { get; set; }
    }
}
