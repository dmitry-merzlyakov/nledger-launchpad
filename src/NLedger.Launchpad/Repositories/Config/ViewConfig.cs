using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.Config
{
    public class ViewConfig
    {
        public bool IsWorkspaceVisible { get; set; } = true;
        public bool IsFileSystemCollapsed { get; set; } = true;
        public bool IsFavoritesCollapsed { get; set; } = true;
        public Guid SelectedFileSystemItem { get; set; }
        public int WorkspaceCardWidth { get; set; } = 270;
    }
}
