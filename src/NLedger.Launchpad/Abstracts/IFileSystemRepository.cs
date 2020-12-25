using NLedger.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Abstracts
{
    public interface IFileSystemRepository
    {
        IFileSystemProvider FileSystemProvider { get; }
        IFileSystemItem InputStream { get; }
        IFileSystemItem RootFolder { get; }

        Task<IFileSystemItem> GetItem(Guid key);
        Task<IEnumerable<IFileSystemItem>> GetDirectory();
        event EventHandler DirectoryChanged;

        Task<IFileSystemItem> CreateFolder(Guid parentFolderKey, string folderName);
        Task<IFileSystemItem> CreateFile(Guid parentFolderKey, string fileName, string content = null);
        Task<IFileSystemItem> RenameFileSystemItem(Guid itemKey, string name);
        Task<IFileSystemItem> DeleteFileSystemItem(Guid itemKey);

        Task<string> GetFileContent(Guid fileKey);
        Task SetFileContent(Guid fileKey, string content);
    }
}
