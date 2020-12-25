using Microsoft.Extensions.Logging;
using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class SampleManager
    {
        public static readonly string SamplesFolderName = "samples";
        public static readonly string SamplesFolderPath = "/" + SamplesFolderName + "/";

        public SampleManager(HttpClient httpClient, IFileSystemRepository fileSystemRepository, IFavoriteRepository favoriteRepository)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));
            if (fileSystemRepository == null)
                throw new ArgumentNullException(nameof(fileSystemRepository));
            if (favoriteRepository == null)
                throw new ArgumentNullException(nameof(favoriteRepository));

            FileSystemRepository = fileSystemRepository;
            FavoriteRepository = favoriteRepository;
            HttpClient = httpClient;
        }

        public IFileSystemRepository FileSystemRepository { get; }
        public IFavoriteRepository FavoriteRepository { get; }
        public HttpClient HttpClient { get; }
        public ILogger<SampleManager> Logger { get; } = Logging.AppLoggingFactory.GetLogger<SampleManager>();

        public async Task<bool> ApplySamples(IEnumerable<SampleItem> samples)
        {
            if (samples == null || !samples.Any())
            {
                Logger.LogDebug("No samples to apply; stopped.");
                return false;
            }

            var hasChanges = false;

            // Download related files

            var files = samples.SelectMany(s => s.Files).Distinct().ToArray();
            Logger.LogDebug($"Queued {files.Length} files to download.");

            var fileContent = new Dictionary<string, string>();
            foreach(var file in files)
            {
                var sampleFile = await HttpClient.GetFromJsonAsync<SampleFile>($"sample-data/{file}.json");
                fileContent.Add(file, sampleFile.Content);
                Logger.LogDebug($"Downloaded {file} content ({sampleFile.Content.Length} characters)");
            }

            // Add files to the file system

            var fileSystemItems = (await FileSystemRepository.GetDirectory()).ToArray();
            var sampleFolder = fileSystemItems.FirstOrDefault(f => f.Kind == FileSystemItemKind.Folder && String.Equals(f.Path, SamplesFolderPath, StringComparison.InvariantCultureIgnoreCase));
            if (sampleFolder == null)
            {
                sampleFolder = await FileSystemRepository.CreateFolder(FileSystemRepository.RootFolder.Key, SamplesFolderName);
                hasChanges = true;
                Logger.LogDebug($"'{SamplesFolderName}' folder created");
            }
            Logger.LogDebug($"'{SamplesFolderName}' folder key is {sampleFolder.Key}");

            var fileKeys = new Dictionary<string, Guid>();
            foreach (var kvFile in fileContent)
            {
                var filePath = SamplesFolderPath + kvFile.Key;
                var sampleFile = fileSystemItems.FirstOrDefault(f => f.Kind == FileSystemItemKind.File && String.Equals(f.Path, filePath, StringComparison.InvariantCultureIgnoreCase));
                if (sampleFile == null)
                {
                    sampleFile = await FileSystemRepository.CreateFile(sampleFolder.Key, kvFile.Key, kvFile.Value);
                    await FileSystemRepository.SetFileContent(sampleFile.Key, kvFile.Value);
                    hasChanges = true;
                    Logger.LogDebug($"File '{kvFile.Key}' is created");
                }
                else
                {
                    var sampleContent = await FileSystemRepository.GetFileContent(sampleFile.Key);
                    if (sampleContent != kvFile.Value)
                    {
                        await FileSystemRepository.SetFileContent(sampleFile.Key, kvFile.Value);
                        hasChanges = true;
                        Logger.LogDebug($"File '{kvFile.Key}' is updated");
                    }
                }
                fileKeys.Add(kvFile.Key, sampleFile.Key);
            }

            // Add favorites

            var favoriteItems = (await FavoriteRepository.GetFavorites()).ToArray();
            foreach(var sample in samples)
            {
                var fileKey = sample.Files.Any() ? fileKeys[sample.Files.First()] : FileSystemRepository.InputStream.Key;

                var favoriteItem = favoriteItems.FirstOrDefault(f => f.Key == sample.SampleID);
                if (favoriteItem == null)
                {
                    favoriteItem = await FavoriteRepository.CreateFavorite(sample.Title, fileKey, sample.Command, sample.SampleID);
                    hasChanges = true;
                    Logger.LogDebug($"Favorite '{sample.SampleID}' is created");
                }
                else
                {
                    if (favoriteItem.Title != sample.Title || favoriteItem.FileKey != fileKey || favoriteItem.Command != sample.Command)
                    {
                        favoriteItem = await FavoriteRepository.EditFavorite(sample.Title, fileKey, sample.Command, sample.SampleID);
                        hasChanges = true;
                        Logger.LogDebug($"Favorite '{sample.SampleID}' is updated");
                    }
                }
            }

            Logger.LogDebug($"Samples are updated; hasChanges={hasChanges}");
            return hasChanges;
        }
    }
}
