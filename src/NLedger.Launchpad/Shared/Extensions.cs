using NLedger.Launchpad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Shared
{
    public static class Extensions
    {
        public static string WorkspaceItemIcon(this WorkspaceItemKindEnum kind)
        {
            switch (kind)
            {
                case WorkspaceItemKindEnum.InputStream: return "oi oi-align-left";
                case WorkspaceItemKindEnum.RootFolder: return "oi oi-home";
                case WorkspaceItemKindEnum.Folder: return "oi oi-folder";
                case WorkspaceItemKindEnum.File: return "oi oi-file";
                case WorkspaceItemKindEnum.Favorite: return "oi oi-book";
                default: throw new InvalidOperationException($"Unknown WorkspaceItemKindEnum value: {kind}");
            }
        }
    }
}
