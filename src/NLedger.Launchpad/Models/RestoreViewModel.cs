using BlazorInputFile;
using NLedger.Launchpad.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public enum InfoMessageKind
    {
        None,
        Error,
        Info,
        Progress
    }

    public class RestoreViewModel
    {
        public string InfoMessage { get; private set; }
        public InfoMessageKind InfoMessageKind { get; private set; } = InfoMessageKind.None;
        public bool HasMessage => InfoMessageKind != InfoMessageKind.None;

        public byte[] Data { get; private set; }

        public async Task HandleFileSelected(IFileListEntry[] files)
        {
            SetProgress();
            await Task.Delay(10);

            try
            {
                var reader = new FileListEntryReader()
                {
                    DataValidation = data => { if (!LocalStorageManager.ValidateZip(data)) throw new InvalidOperationException("Invalid archive format"); }
                };

                var file = files?.FirstOrDefault();
                Data = await reader.HandleFile(file);
                SetInfo(file.Name);
            }
            catch (Exception ex)
            {
                SetError(ex.Message);
                Data = null;
            }
        }

        public bool Validate()
        {
            if (Data == null && !HasMessage)
                SetError("No file");

            return Data != null;
        }

        private void SetProgress()
        {
            InfoMessage = "Checking...";
            InfoMessageKind = InfoMessageKind.Progress;
        }

        private void SetInfo(string fileName)
        {
            InfoMessage = $"Selected file: {fileName}";
            InfoMessageKind = InfoMessageKind.Info;
        }

        private void SetError(string message)
        {
            InfoMessage = message;
            InfoMessageKind = InfoMessageKind.Error;
        }

    }
}
