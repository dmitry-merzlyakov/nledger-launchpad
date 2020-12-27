using Microsoft.JSInterop;
using NLedger.Launchpad.Abstracts;
using NLedger.Launchpad.Repositories;
using NLedger.Launchpad.Repositories.Config;
using NLedger.Launchpad.Repositories.Favorites;
using NLedger.Launchpad.Repositories.FileSystem;
using NLedger.Launchpad.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class AppViewModel
    {
        public AppViewModel(LocalStorageRepository localStorageRepository)
        {
            if (localStorageRepository == null)
                throw new ArgumentNullException(nameof(localStorageRepository));

            LocalStorageRepository = localStorageRepository;
            FileSystemRepository = new FileSystemRepository(localStorageRepository);
            FavoriteRepository = new FavoriteRepository(localStorageRepository);
            ConfigRepository = new ConfigRepository(localStorageRepository);

            InputStreamItem = new WorkspaceItemModel(FileSystemRepository.InputStream.Key, WorkspaceItemKindEnum.InputStream, FileSystemRepository.InputStream.Name);

            CommandExecutionModel = new CommandExecutionModel(
                FileSystemRepository,
                getSourceKey: () => Selected,
                addCommandToHistory: AddCommandToHistory,
                getEnvironmentalConfig: GetEnvironmentalConfig,
                saveTextEditorContent: SaveTextEditorContent
            );
        }

        public LocalStorageRepository LocalStorageRepository { get; }
        public IFileSystemRepository FileSystemRepository { get; }
        public IFavoriteRepository FavoriteRepository { get; }
        public IConfigRepository ConfigRepository { get; }

        public CommandExecutionModel CommandExecutionModel { get; private set; }
        public WorkspaceItemModel InputStreamItem { get; }

        public ViewConfig ViewConfig { get; private set; }

        public async Task Initialize()
        {
            ViewConfig = await ConfigRepository.GetViewConfig();
            if (ViewConfig.SelectedFileSystemItem == default(Guid))
                ViewConfig.SelectedFileSystemItem = InputStreamItem.Key;
        }

        public async Task<IEnumerable<WorkspaceItemModel>> GetFileSystemItems()
        {
            return (await FileSystemRepository.GetDirectory()).
                Select(f => new WorkspaceItemModel(f.Key, f.Kind.ToWorkspaceItemKind(), f.Path)).OrderBy(m => m.Title).ToList();
        }

        public async Task<IEnumerable<WorkspaceItemModel>> GetFavoriteItems()
        {
            return (await FavoriteRepository.GetFavorites()).
                Select(f => new WorkspaceItemModel(f.Key, WorkspaceItemKindEnum.Favorite, f.Title)).OrderBy(m => m.Title).ToList();
        }

        public async Task<TextEditorModel> GetTextEditorModel()
        {
            string text = String.Empty;

            if (Selected != null && LocalStorageRepository.LocalStorageService != null)
                text = await FileSystemRepository.GetFileContent(Selected);

            if (String.IsNullOrEmpty(text))
                text = DummySourceText;

            return new TextEditorModel()
            {
                DataSourceTitle = (await FileSystemRepository.GetItem(Selected)).Path,
                Text = text
            };
        }

        public async Task<FavoriteItemViewModel> GetFavoriteItemViewModel(Guid key = default(Guid))
        {
            var fileItems = new List<WorkspaceItemModel>() { InputStreamItem };
            fileItems.AddRange(await GetFileSystemItems());

            if (key == default(Guid))
            {
                return new FavoriteItemViewModel(fileItems)
                {
                    FileKey = Selected,
                    Command = CommandExecutionModel?.CommandText
                };
            }
            else
            {
                var item = await FavoriteRepository.GetItem(key);
                return new FavoriteItemViewModel(fileItems, item.Key)
                {
                    Title = item.Title,
                    FileKey = item.FileKey,
                    Command = item.Command,
                };
            }
        }

        public async Task SaveFavoriteItemViewModel(FavoriteItemViewModel model)
        {
            if (model.Key == default(Guid))
                await FavoriteRepository.CreateFavorite(model.Title, model.FileKey, model.Command);
            else
                await FavoriteRepository.EditFavorite(model.Title, model.FileKey, model.Command, model.Key);

            SelectedChanged?.Invoke();
        }

        public async Task ExecuteFavoriteItemViewModel(Guid key)
        {
            var item = await FavoriteRepository.GetItem(key);

            CommandExecutionModel.CommandText = item.Command;

            if (Selected != item.FileKey)
                await SetSelected(item.FileKey);

            SelectedChanged?.Invoke();

            await CommandExecutionModel.RunCommand();
        }

        public async Task DeleteFavoriteItem(Guid key)
        {
            await FavoriteRepository.DeleteFavorite(key);
            SelectedChanged?.Invoke();
        }

        public bool IsWorkspaceItemSelected(WorkspaceItemModel workspaceItemModel)
        {
            return workspaceItemModel.Key == Selected;
        }

        public async Task SelectWorkspaceItem(Guid key, bool isFavorite = false)
        {
            if (isFavorite)
                await ExecuteFavoriteItemViewModel(key);
            else
            {
                var prevSelected = Selected;
                await SetSelected(key);

                if (prevSelected != default(Guid) && prevSelected != Selected && GetTextEditorText != null)
                {
                    var content = await GetTextEditorText.Invoke();
                    await FileSystemRepository.SetFileContent(prevSelected, content);
                }

                SelectedChanged?.Invoke();
            }
        }

        public async Task SaveTextEditorContent()
        {
            var content = await GetTextEditorText.Invoke();
            await FileSystemRepository.SetFileContent(Selected, content);
        }

        private Guid Selected => ViewConfig.SelectedFileSystemItem;
        public event Func<Task> SelectedChanged;

        public async Task SetSelected(Guid selected)
        {
            ViewConfig.SelectedFileSystemItem = selected;
            await ConfigRepository.SetViewConfig(ViewConfig);
        }

        public Func<ValueTask<string>> GetTextEditorText { get; set; }

        public async Task CreateFolder(Guid parentFolderKey, string folderName)
        {
            await FileSystemRepository.CreateFolder(parentFolderKey, folderName);
            SelectedChanged?.Invoke();
        }

        public async Task CreateFile(Guid parentFolderKey, string fileName)
        {
            var file = await FileSystemRepository.CreateFile(parentFolderKey, fileName);
            await SelectWorkspaceItem(file.Key);
        }

        public async Task RenameFileSystemItem(Guid itemKey, string name)
        {
            await FileSystemRepository.RenameFileSystemItem(itemKey, name);
            SelectedChanged?.Invoke();
        }

        public async Task DeleteFileSystemItem(Guid itemKey)
        {
            var deleted = await FileSystemRepository.DeleteFileSystemItem(itemKey);
            if (Selected == deleted.Key)
                await SetSelected(InputStreamItem.Key);
            SelectedChanged?.Invoke();
        }

        public async Task<string> GetFileSystemItemName(Guid itemKey)
        {
            return (await FileSystemRepository.GetItem(itemKey)).Name;
        }


        private CommandHistoryList CommandHistoryList { get; set; }

        public async Task<CommandHistoryList> GetCommandHistoryList()
        {
            return CommandHistoryList ?? (CommandHistoryList = await ConfigRepository.GetCommandHistoryList());
        }

        public async Task<IEnumerable<string>> GetHistory()
        {
            var list = (await GetCommandHistoryList()).Commands;
            return list.Reverse().ToArray();
        }

        public async Task AddCommandToHistory(string command)
        {
            var commandHistoryList = await GetCommandHistoryList();
            if (commandHistoryList.AddCommand(command))
                await ConfigRepository.SetCommandHistoryList(commandHistoryList);
        }

        public async Task SelectHistoryCommand(string command)
        {
            await CommandExecutionModel.SetCommandText(command);
        }


        private EnvironmentalConfig EnvironmentalConfig { get; set; }

        public async Task<EnvironmentalConfig> GetEnvironmentalConfig()
        {
            return EnvironmentalConfig ?? (EnvironmentalConfig = await ConfigRepository.GetEnvironmentalConfig());
        }

        public async Task<EnvironmentalConfigViewModel> GetEnvironmentalConfigViewModel()
        {
            var config = await GetEnvironmentalConfig();
            return new EnvironmentalConfigViewModel()
            {
                IsAtty = config.IsAtty,
                TimeZoneId = config.TimeZone.Id,
                OutputEncoding = Encoding.GetEncodings().SingleOrDefault(c => c.Name == config.OutputEncoding.WebName).CodePage,
                EnvironmentVariables = config.EnvironmentVariables.Select(kv => new EnvironmentVariableModel() { Name = kv.Key, Value = kv.Value }).ToList(),
            };
        }

        public async Task SaveEnvironmentalConfigViewModel(EnvironmentalConfigViewModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            await ConfigRepository.SetEnvironmentalConfig(EnvironmentalConfig = new EnvironmentalConfig(model.IsAtty, 
                TimeZoneInfo.FindSystemTimeZoneById(model.TimeZoneId),
                Encoding.GetEncoding(model.OutputEncoding), model.EnvironmentVariables.ToDictionary(v => v.Name, v => v.Value)));
        }


        public async Task ApplySamples(HttpClient httpClient, IEnumerable<SampleItem> samples)
        {
            var sampleManager = new SampleManager(httpClient, FileSystemRepository, FavoriteRepository);
            var hasChanges = await sampleManager.ApplySamples(samples);
            if (hasChanges)
                SelectedChanged?.Invoke();
        }

        public async Task BackupWorkspace(IJSRuntime js, string fileName)
        {
            var localStorageManager = new LocalStorageManager(LocalStorageRepository);
            var backup = await localStorageManager.ExportToZip();
            await js.SaveAs(fileName, backup);
        }

        public async Task ResetWorkspace(IJSRuntime js)
        {
            var localStorageManager = new LocalStorageManager(LocalStorageRepository);
            await localStorageManager.Clear();
            await js.ReloadApplication();
        }

        public async Task RestoreWorkspace(IJSRuntime js, byte[] data)
        {
            var localStorageManager = new LocalStorageManager(LocalStorageRepository);
            await localStorageManager.Clear();
            await localStorageManager.ImportFromZip(data);
            await js.ReloadApplication();
        }

        public async Task WorkspaceVisibleToggle()
        {
            ViewConfig.IsWorkspaceVisible = !ViewConfig.IsWorkspaceVisible;
            await ConfigRepository.SetViewConfig(ViewConfig);
            await IsWorkspaceVisibleChanged?.Invoke();
        }

        public bool IsWorkspaceVisible => ViewConfig.IsWorkspaceVisible;
        public event Func<Task> IsWorkspaceVisibleChanged;

        public bool IsFileSystemCollapsed => ViewConfig.IsFileSystemCollapsed;
        public async Task SetIsFileSystemCollapsed(bool value)
        {
            ViewConfig.IsFileSystemCollapsed = value;
            await ConfigRepository.SetViewConfig(ViewConfig);
        }

        public bool IsFavoritesCollapsed => ViewConfig.IsFavoritesCollapsed;
        public async Task SetIsFavoritesCollapsed(bool value)
        {
            ViewConfig.IsFavoritesCollapsed = value;
            await ConfigRepository.SetViewConfig(ViewConfig);
        }

        public int WorkspaceCardWidth => ViewConfig.WorkspaceCardWidth;
        public async Task SetWorkspaceCardWidth(int value)
        {
            ViewConfig.WorkspaceCardWidth = value;
            await ConfigRepository.SetViewConfig(ViewConfig);
        }

        private static readonly string DummySourceText = @"
2009/10/29 (XFER) Panera Bread
    Expenses:Food               $4.50
    Assets:Checking

2009/10/30 (DEP) Pay day!
    Assets:Checking            $20.00
    Income

2009/10/30 (XFER) Panera Bread
    Expenses:Food               $4.50
    Assets:Checking

2009/10/31 (559385768438A8D7) Panera Bread
    Expenses:Food               $4.50
    Liabilities:Credit Card
";


    }
}
