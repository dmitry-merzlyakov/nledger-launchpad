using Microsoft.Extensions.Logging;
using NLedger.Abstracts;
using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public class FileSystemRepository : IFileSystemRepository
    {
        public static readonly string LocalStorageDirectoryFileName = "NLedger.Launchpad.FileSystem.Directory";
        public static readonly string LocalStorageFileNameTemplate = "NLedger.Launchpad.FileSystem.File.";

        public FileSystemRepository(ILocalStorageRepository localStorageRepository)
        {
            if (localStorageRepository == null)
                throw new ArgumentNullException(nameof(localStorageRepository));

            LocalStorageRepository = localStorageRepository;

            AddFileSystemItem(FileSystemInputStream);
            AddFileSystemItem(FileSystemRootFolder);
        }

        public ILocalStorageRepository LocalStorageRepository { get; }
        public ILogger<FileSystemRepository> Logger { get; } = Logging.AppLoggingFactory.GetLogger<FileSystemRepository>();

        public IFileSystemItem InputStream => FileSystemInputStream;
        public IFileSystemItem RootFolder => FileSystemRootFolder;

        public IFileSystemProvider FileSystemProvider => throw new NotImplementedException();

        public event EventHandler DirectoryChanged;

        public async Task<IFileSystemItem> CreateFile(Guid parentFolderKey, string fileName, string content = null)
        {
            var item = (await GetItem(parentFolderKey)) as FileSystemFolder;
            if (item == null)
                throw new InvalidOperationException($"Parent folder {parentFolderKey} not found.");

            return await ProcessDirectoryChanges(AddFileSystemItem(new FileSystemFile(Guid.NewGuid(), fileName, item)));
        }

        public async Task<IFileSystemItem> CreateFolder(Guid parentFolderKey, string folderName)
        {
            var item = (await GetItem(parentFolderKey)) as FileSystemFolder;
            if (item == null)
                throw new InvalidOperationException($"Parent folder {parentFolderKey} not found.");

            return await ProcessDirectoryChanges(AddFileSystemItem(new FileSystemFolder(Guid.NewGuid(), folderName, item)));
        }

        public async Task<IFileSystemItem> DeleteFileSystemItem(Guid itemKey)
        {
            var item = await GetItem(itemKey) as FileSystemItem;
            if (item == null)
                throw new InvalidOperationException($"Item {itemKey} not found.");

            if (!item.HasParent)
                throw new InvalidOperationException($"Item {itemKey} cannot be deleted because it does not have a parent folder.");

            item.ParentFolder.RemoveChildItem(item);
            RemoveFileSystemItems(item.GetAllItems());

            return await ProcessDirectoryChanges(item);
        }

        public async Task<IEnumerable<IFileSystemItem>> GetDirectory()
        {
            await EnsureInitialized();
            return FileSystemItems.Values.Where(f => f.Kind != FileSystemItemKind.InputStream).ToArray();
        }

        public async Task<string> GetFileContent(Guid fileKey)
        {
            string content;
            if (!FileContent.TryGetValue(fileKey, out content))
            {
                content = await LocalStorageRepository.GetItem(LocalStorageFileNameTemplate + fileKey.ToString());
                FileContent.Add(fileKey, content ?? String.Empty);
            }
            return content;
        }

        public async Task<IFileSystemItem> GetItem(Guid key)
        {
            await EnsureInitialized();

            FileSystemItem item;
            if (FileSystemItems.TryGetValue(key, out item))
                return item;

            return null;
        }

        public async Task<IFileSystemItem> RenameFileSystemItem(Guid itemKey, string name)
        {
            var item = await GetItem(itemKey) as FileSystemItem;
            if (item == null)
                throw new InvalidOperationException($"Item {itemKey} not found.");

            if (!item.HasParent)
                throw new InvalidOperationException($"Item {itemKey} cannot be renamed (it should have a parent folder).");

            FileSystemItem.ValidateName(name);
            if (item.ParentFolder.ContainsName(name))
                throw new InvalidOperationException($"Parent folder already contains name '{name}'.");

            item.ParentFolder.RemoveChildItem(item);
            item.Rename(name);
            item.ParentFolder.AddChildItem(item);

            return await ProcessDirectoryChanges(item);
        }

        public async Task SetFileContent(Guid fileKey, string content)
        {
            FileContent[fileKey] = content;
            await LocalStorageRepository.SetItem(LocalStorageFileNameTemplate + fileKey.ToString(), content);
        }

        private async Task EnsureInitialized()
        {
            if (!IsInitialized)
            {
                try
                {
                    var serializedDirectory = await LocalStorageRepository.GetItem(LocalStorageDirectoryFileName);
                    if (!String.IsNullOrWhiteSpace(serializedDirectory))
                    {
                        var dtos = new FileSystemDirectorySerializer().Deserialize(serializedDirectory).ToList();
                        await RestoreFromDto(dtos, RootFolder.Key);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Initialization error");
                }
                IsInitialized = true;
            }
        }

        private async Task RestoreFromDto(IList<FileSystemItemDto> dtos, Guid parentKey)
        {
            FileSystemItem parentItem;
            if (!FileSystemItems.TryGetValue(parentKey, out parentItem))
                throw new InvalidOperationException($"Parent folder {parentKey} not found.");

            var parentFolder = parentItem as FileSystemFolder;
            if (parentFolder == null)
                throw new InvalidOperationException($"Key {parentKey} does not refer to a folder.");

            foreach (var item in dtos.Where(d => d.ParentKey == parentKey))
            {
                switch(item.Kind)
                {
                    case FileSystemItemKind.Folder:
                        AddFileSystemItem(new FileSystemFolder(item.Key, item.Name, parentFolder));
                        await RestoreFromDto(dtos, item.Key);
                        break;

                    case FileSystemItemKind.File:
                        AddFileSystemItem(new FileSystemFile(item.Key, item.Name, parentFolder));
                        break;

                    default:
                        throw new InvalidOperationException("Expected files and folders only");
                }
            }
        }

        private T AddFileSystemItem<T>(T item) where T : FileSystemItem
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (FileSystemItems.ContainsKey(item.Key))
                throw new ArgumentException($"Item {item.Key} is already added");

            FileSystemItems.Add(item.Key, item);
            return item;
        }

        private void RemoveFileSystemItems(IEnumerable<FileSystemItem> items)
        {
            items.ToList().ForEach(p => FileSystemItems.Remove(p.Key));
        }

        private async Task<T> ProcessDirectoryChanges<T>(T item) where T : FileSystemItem
        {
            var serializedDirectory = new FileSystemDirectorySerializer().Serialize(FileSystemItems.Values.Where(p => p.HasParent));
            await LocalStorageRepository.SetItem(LocalStorageDirectoryFileName, serializedDirectory);

            DirectoryChanged?.Invoke(this, EventArgs.Empty);
            return item;
        }

        private bool IsInitialized { get; set; }

        private readonly FileSystemInputStream FileSystemInputStream = new FileSystemInputStream();
        private readonly FileSystemRootFolder FileSystemRootFolder = new FileSystemRootFolder();
        private readonly IDictionary<Guid, FileSystemItem> FileSystemItems = new Dictionary<Guid, FileSystemItem>();
        private readonly IDictionary<Guid, string> FileContent = new Dictionary<Guid, string>();
    }
}
