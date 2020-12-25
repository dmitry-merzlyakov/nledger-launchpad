using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{


    public class WorkspaceItemModel
    {
        public WorkspaceItemModel(Guid key, WorkspaceItemKindEnum kind, string title)
        {
            Key = key;
            Kind = kind;
            Title = title;
        }

        public Guid Key { get; private set; }
        public string Title { get; private set; }
        public WorkspaceItemKindEnum Kind { get; private set; }

        public bool IsFavorite => Kind == WorkspaceItemKindEnum.Favorite;

        public bool CanAddFolder => Kind == WorkspaceItemKindEnum.RootFolder || Kind == WorkspaceItemKindEnum.Folder;
        public bool CanAddFile => Kind == WorkspaceItemKindEnum.RootFolder || Kind == WorkspaceItemKindEnum.Folder;
        public bool CanEdit => Kind == WorkspaceItemKindEnum.Folder || Kind == WorkspaceItemKindEnum.File || Kind == WorkspaceItemKindEnum.Favorite;
        public bool CanDelete => Kind == WorkspaceItemKindEnum.Folder || Kind == WorkspaceItemKindEnum.File || Kind == WorkspaceItemKindEnum.Favorite;
        public bool IsDisabled => Kind == WorkspaceItemKindEnum.Folder || Kind == WorkspaceItemKindEnum.RootFolder;
        public bool IsClickable => Kind == WorkspaceItemKindEnum.File || Kind == WorkspaceItemKindEnum.InputStream || Kind == WorkspaceItemKindEnum.Favorite;
    }
}
