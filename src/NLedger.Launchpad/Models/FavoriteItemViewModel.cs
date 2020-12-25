using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class FavoriteItemViewModel
    {
        public FavoriteItemViewModel(IEnumerable<WorkspaceItemModel> fileSystemItems, Guid key = default(Guid))
        {
            if (fileSystemItems == null)
                throw new ArgumentNullException(nameof(fileSystemItems));

            Key = key;

            FileSystemItems = fileSystemItems.Where(f => f.Kind == WorkspaceItemKindEnum.File || f.Kind == WorkspaceItemKindEnum.InputStream).ToArray();

            var inputStream = FileSystemItems.FirstOrDefault(f => f.Kind == WorkspaceItemKindEnum.InputStream);
            if (inputStream == null)
                throw new InvalidOperationException($"Input stream is not presented in the list of file system items");
            InputStreamKey = inputStream.Key;
        }

        public Guid Key { get; }
        public string Title { get; set; }
        public Guid FileKey { get; set; }
        public string Command { get; set; }

        public Guid InputStreamKey { get; }
        public IEnumerable<WorkspaceItemModel> FileSystemItems { get; }

        public bool IsNew => Key == default(Guid);

    }
}
