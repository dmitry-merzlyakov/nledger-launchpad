using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public static class Extensions
    {
        public static WorkspaceItemKindEnum ToWorkspaceItemKind (this FileSystemItemKind fileSystemItemKind)
        {
            switch(fileSystemItemKind)
            {
                case FileSystemItemKind.RootFolder: return WorkspaceItemKindEnum.RootFolder;
                case FileSystemItemKind.Folder: return WorkspaceItemKindEnum.Folder;
                case FileSystemItemKind.File: return WorkspaceItemKindEnum.File;
                case FileSystemItemKind.InputStream: return WorkspaceItemKindEnum.InputStream;
                default: throw new InvalidOperationException($"Unknown FileSystemItemKind value {fileSystemItemKind}");
            }
        }
    }
}
