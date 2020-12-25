using Microsoft.AspNetCore.Components;
using NLedger.Launchpad.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Services
{
    public enum DialogButtonKind
    {
        None,
        Close,
        Create,
        Save,
        Rename,
        Delete,
        Download,
        Reset,
        Restore,
        Cancel
    }

    public enum DialogKind
    {
        Success,
        Info,
        Warning,
        Danger,
        Primary,
        Secondary,
        Light,
        Dark
    }

    public class DialogValidationResult
    {
        public bool IsValid { get; set; } = true;
    }

    public enum DialogPlaceholderCommandEnum
    {
        Show,
        Close
    }

    public class DialogPlaceholderStateChange
    {
        public DialogPlaceholderCommandEnum Command { get; set; }
        public Type CommponentType { get; set; }
    }

    public class DialogService
    {
        public event Func<DialogPlaceholderStateChange, Task> OnDialogPlaceholderStateChange;

        public object CurrentModel { get; private set; }

        public async Task OpenDialog<T>() where T : ComponentBase
        {
            CurrentModel = null;
            await OpenDialog<T>(null);
        }

        public async Task OpenDialog<T>(object model) where T : ComponentBase
        {
            CurrentModel = model;
            await OnDialogPlaceholderStateChange.Invoke(new DialogPlaceholderStateChange() { Command = DialogPlaceholderCommandEnum.Show, CommponentType = typeof(T) });
        }

        public async Task CloseDialog()
        {
            await OnDialogPlaceholderStateChange.Invoke(new DialogPlaceholderStateChange() { Command = DialogPlaceholderCommandEnum.Close });
        }
    }
}
