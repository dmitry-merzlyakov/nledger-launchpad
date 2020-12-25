using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public abstract class FileSystemItem : IFileSystemItem
    {
        public static readonly string DirectorySeparator = "/";
        public static readonly string CurrentDirectoryReference = ".";
        public static readonly string ParentDirectoryReference = "..";

        public FileSystemItem(Guid key, string name, FileSystemFolder parentFolder = null)
        {
            if (key == default(Guid))
                throw new ArgumentNullException(nameof(key));

            ValidateFileSystemName(name);

            Key = key;
            Name = name;

            if (HasParent && parentFolder == null)
                throw new ArgumentNullException(nameof(parentFolder));
            if (!HasParent && parentFolder != null)
                throw new ArgumentException($"{Kind} cannot have a parent folder");

            ParentFolder = parentFolder;

            if (HasParent)
                ParentFolder.AddChildItem(this);
        }

        public Guid Key { get; }

        public string Name { get; private set; }

        public abstract FileSystemItemKind Kind { get; }

        public FileSystemFolder ParentFolder { get; }

        public virtual string Path => (ParentFolder?.Path ?? String.Empty) + Name;

        public virtual bool HasParent => Kind == FileSystemItemKind.File || Kind == FileSystemItemKind.Folder;

        public void Rename(string name)
        {
            ValidateName(name);
            Name = name;
        }

        public virtual IEnumerable<FileSystemItem> GetAllItems()
        {
            return new FileSystemItem[] { this };
        }

        public static void ValidateName(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (name.Contains(DirectorySeparator) || name == CurrentDirectoryReference || name == ParentDirectoryReference)
                throw new ArgumentException($"Incorrect name: '{name}'");
        }

        protected virtual void ValidateFileSystemName(string name)
        {
            ValidateName(name);
        }
    }
}
