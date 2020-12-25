using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Abstracts
{
    public interface IFavoriteRepository
    {
        Task<IFavoriteItem> GetItem(Guid key);
        Task<IEnumerable<IFavoriteItem>> GetFavorites();
        event EventHandler FavoritesChanged;

        Task<IFavoriteItem> CreateFavorite(string title, Guid fileKey, string command, Guid key = default(Guid));
        Task<IFavoriteItem> EditFavorite(string title, Guid fileKey, string command, Guid key);
        Task<IFavoriteItem> DeleteFavorite(Guid itemKey);
    }
}
