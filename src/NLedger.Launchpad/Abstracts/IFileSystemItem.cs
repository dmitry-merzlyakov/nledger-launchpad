using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Abstracts
{
    public enum FileSystemItemKind
    {
        RootFolder = 0,
        Folder = 1,
        File = 2,
        InputStream = 3
    }

    public interface IFileSystemItem
    {
        Guid Key { get; }
        string Name { get; }
        string Path { get; }
        FileSystemItemKind Kind { get; }
    }
}
