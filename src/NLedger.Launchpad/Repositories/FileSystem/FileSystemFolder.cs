using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public class FileSystemFolder : FileSystemItem
    {
        public FileSystemFolder(Guid key, string name, FileSystemFolder parentFolder) 
            : base(key, name, parentFolder)
        { }

        public override FileSystemItemKind Kind => FileSystemItemKind.Folder;

        public override string Path => (ParentFolder?.Path ?? String.Empty) + Name +  DirectorySeparator;

        public FileSystemItem AddChildItem(FileSystemItem fileSystemItem)
        {
            if (fileSystemItem == null)
                throw new ArgumentNullException(nameof(fileSystemItem));

            if (Children.ContainsKey(fileSystemItem.Name))
                throw new InvalidOperationException($"Parent folder already contains an item with name {fileSystemItem.Name}");

            Children.Add(fileSystemItem.Name, fileSystemItem);
            return fileSystemItem;
        }

        public FileSystemItem RemoveChildItem(FileSystemItem fileSystemItem)
        {
            if (fileSystemItem == null)
                throw new ArgumentNullException(nameof(fileSystemItem));

            Children.Remove(fileSystemItem.Name);
            return fileSystemItem;
        }

        public bool ContainsName(string name)
        {
            return Children.ContainsKey(name);
        }

        public override IEnumerable<FileSystemItem> GetAllItems()
        {
            var list = base.GetAllItems().ToList();
            foreach(var child in Children.Values)
                list.AddRange(child.GetAllItems());

            return list;
        }

        private readonly IDictionary<string, FileSystemItem> Children = new Dictionary<string, FileSystemItem>(StringComparer.InvariantCultureIgnoreCase);
    }
}
