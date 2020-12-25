using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Utility
{
    public class FileListEntryReader
    {
        public int MaxFileSize { get; set; } = 1048576;
        public string FileType { get; set; } = "application/x-zip-compressed";
        public Action<byte[]> DataValidation { get; set; }

        public async Task<byte[]> HandleFile(IFileListEntry entry)
        {
            if (entry == null)
                throw new InvalidOperationException("No file");

            if (entry.Size == 0)
                throw new InvalidOperationException("Empty file");

            if (entry.Size > MaxFileSize)
                throw new InvalidOperationException($"File size ({entry.Size}) exceeds max allowed ({MaxFileSize})");

            if (entry.Type != FileType)
                throw new InvalidOperationException($"Unexpected file type {entry.Type}; ({FileType} expected)");

            byte[] data = null;

            using (var ms = await entry.ReadAllAsync(1048576))
                data = ms.ToArray();

            if (data == null || data.Length == 0)
                throw new InvalidOperationException("No Data");

            DataValidation?.Invoke(data);

            return data;
        }

    }
}
