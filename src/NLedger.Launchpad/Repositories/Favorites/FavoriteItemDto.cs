using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Favorites
{
    public class FavoriteItemDto
    {
        public Guid Key { get; set; }
        public string Title { get; set; }
        public Guid FileKey { get; set; }
        public string Command { get; set; }
    }

    public class FavoriteItemDtoContainer
    {
        public IList<FavoriteItemDto> Items { get; set; }
    }
}
