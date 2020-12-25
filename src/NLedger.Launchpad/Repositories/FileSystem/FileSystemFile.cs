using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public class FileSystemFile : FileSystemItem
    {
        public FileSystemFile(Guid key, string name, FileSystemFolder parentFolder)
            : base(key, name, parentFolder)
        { }

        public override FileSystemItemKind Kind => FileSystemItemKind.File;
    }
}
