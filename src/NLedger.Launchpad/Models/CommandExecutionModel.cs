using NLedger.Abstracts.Impl;
using NLedger.Launchpad.Abstracts;
using NLedger.Launchpad.Repositories;
using NLedger.Launchpad.Repositories.Config;
using NLedger.Launchpad.Repositories.FileSystem;
using NLedger.Scopus;
using NLedger.Utility;
using NLedger.Utility.ServiceAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class CommandExecutionModel
    {
        public CommandExecutionModel(IFileSystemRepository fileSystemRepository, Func<Guid> getSourceKey, Func<string, Task> addCommandToHistory, Func<Task<EnvironmentalConfig>> getEnvironmentalConfig, Func<Task> saveTextEditorContent)
        {
            FileSystemRepository = fileSystemRepository ?? throw new ArgumentNullException(nameof(fileSystemRepository));
            FileSystemProviderConnector = new FileSystemProviderConnector(fileSystemRepository);
            GetSourceKey = getSourceKey ?? throw new ArgumentNullException(nameof(getSourceKey));
            AddCommandToHistory = addCommandToHistory ?? throw new ArgumentNullException(nameof(addCommandToHistory));
            GetEnvironmentalConfig = getEnvironmentalConfig ?? throw new ArgumentNullException(nameof(getEnvironmentalConfig));
            SaveTextEditorContent = saveTextEditorContent ?? throw new ArgumentNullException(nameof(saveTextEditorContent));
        }

        public string CommandText { get; set; }
        public string OutputText { get; private set; } = GetVersionInfo();
        public string ErrorText { get; private set; }
        public int ResultCode { get; private set; } = -1;
        public bool IsExecuting { get; private set; }
        public event Func<Task> OnChanged;

        public IFileSystemRepository FileSystemRepository { get; }
        public FileSystemProviderConnector FileSystemProviderConnector { get; }
        public Func<Guid> GetSourceKey { get; }
        public Func<string, Task> AddCommandToHistory { get; }
        public Func<Task<EnvironmentalConfig>> GetEnvironmentalConfig { get; }
        public Func<Task> SaveTextEditorContent { get; }

        public async Task RunCommand()
        {
            IsExecuting = true;
            try
            {
                await OnChanged?.Invoke();
                await Task.Delay(10); // Refresh UI
                await ExecuteCommand(CommandText);
            }
            finally
            {
                IsExecuting = false;
                await OnChanged?.Invoke();
            }
        }

        public async Task SetCommandText(string command)
        {
            CommandText = command;
            await OnChanged?.Invoke();
        }

        private async Task ExecuteCommand(string command)
        {
            if (command == "#test-color")
            {
                OutputText = GetTestColor();
                return;
            }

            await SaveTextEditorContent();
            await AddCommandToHistory(command);

            var sourceKey = GetSourceKey();
            var config = await GetEnvironmentalConfig();
            var fileSystem = await FileSystemProviderConnector.GetProvider();  // TODO set current folder, set encoding
            
            var engine = new ServiceEngine(
                configureContext: context => 
                {
                    context.IsAtty = config.IsAtty;
                    context.TimeZone = config.TimeZone;
                    context.SetEnvironmentVariables(config.EnvironmentVariables);
                },
                createCustomProvider: mem =>
                {
                    mem.Attach(w => new MemoryAnsiTextWriter(w));
                    return new ApplicationServiceProvider(
                        fileSystemProviderFactory: () => fileSystem,
                        virtualConsoleProviderFactory: () => new VirtualConsoleProvider(mem.ConsoleInput, mem.ConsoleOutput, mem.ConsoleError));
                });

            ServiceSession session;
            if (sourceKey == FileSystemRepository.InputStream.Key)
            {
                session = engine.CreateSession("-f /dev/stdin", await FileSystemRepository.GetFileContent(sourceKey));
            }
            else
            {
                session = engine.CreateSession($"-f {(await FileSystemRepository.GetItem(sourceKey)).Path}");
            }

            var response = session.ExecuteCommand(command);

            OutputText = response.OutputText;
            ErrorText = response.ErrorText;
            ResultCode = response.Status;

            // TODO - save possible changes in FS
            //if (fileSystem.HasChanges())
            //    await FileSystemProviderConnector.ApplyChanges();
        }

        private static string GetVersionInfo()
        {
            // TODO - should be moved to API
            return String.Format(GlobalScope.ShowVersionInfoTemplate, VersionInfo.NLedgerVersion, VersionInfo.Ledger_VERSION_MAJOR, VersionInfo.Ledger_VERSION_MINOR, VersionInfo.Ledger_VERSION_PATCH, VersionInfo.Ledger_VERSION_DATE);
        }

        private string GetTestColor()
        {
            var sb = new StringBuilder();
            for(var bgColor=-1; bgColor<16; bgColor++)
                for (var fgColor = -1; fgColor < 16; fgColor++)
                {
                    var bgClass = bgColor >= 0 ? $"bg{bgColor}" : String.Empty;
                    var bgText = bgColor >= 0 ? ((ConsoleColor)bgColor).ToString() : "Not set";

                    var fgClass = fgColor >= 0 ? $"fg{fgColor}" : String.Empty;
                    var fgText = fgColor >= 0 ? ((ConsoleColor)fgColor).ToString() : "Not set";

                    sb.AppendLine($"<span class='{bgClass} {fgClass}'>Test Line (0123456789) Color: {fgText,15} Background: {bgText}</span>");
                }
            return sb.ToString();
        }

    }
}
