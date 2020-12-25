using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public class FileSystemRootFolder : FileSystemFolder
    {
        public static readonly Guid RootFolderKey = new Guid("{C682C0DC-E73B-4AA7-B1D7-A1BE620EF884}");

        public FileSystemRootFolder()
            : base(RootFolderKey, DirectorySeparator, null)
        { }

        public override FileSystemItemKind Kind => FileSystemItemKind.RootFolder;

        public override string Path => Name;

        protected override void ValidateFileSystemName(string name)
        { }
    }
}
