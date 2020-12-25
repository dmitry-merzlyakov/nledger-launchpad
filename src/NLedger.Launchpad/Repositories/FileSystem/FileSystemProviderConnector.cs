using NLedger.Abstracts;
using NLedger.Launchpad.Abstracts;
using NLedger.Utility.ServiceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public class FileSystemProviderConnector
    {
        public FileSystemProviderConnector(IFileSystemRepository fileSystemRepository)
        {
            FileSystemRepository = fileSystemRepository ?? throw new ArgumentNullException(nameof(fileSystemRepository));
        }

        public IFileSystemRepository FileSystemRepository { get; }

        public async Task<MemoryFileSystemProvider> GetProvider()
        {
            var provider = new MemoryFileSystemProvider();
            var items = await FileSystemRepository.GetDirectory();

            foreach(var item in items.OrderBy(i => i.Path))
            {
                if (item.Kind == Abstracts.FileSystemItemKind.Folder)
                {
                    provider.CreateFolder(item.Path);
                }
                else if (item.Kind == Abstracts.FileSystemItemKind.File)
                {
                    var content = await FileSystemRepository.GetFileContent(item.Key);
                    provider.AppendAllText(item.Path, content);
                }
            }

            return provider;
        }
    }
}
