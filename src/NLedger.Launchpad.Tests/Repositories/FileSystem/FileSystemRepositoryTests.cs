using Microsoft.Extensions.Logging;
using NLedger.Launchpad.Abstracts;
using NLedger.Launchpad.Logging;
using NLedger.Launchpad.Repositories.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NLedger.Launchpad.Tests.Repositories.FileSystem
{
    public class FileSystemRepositoryTests
    {
        [Fact]
        public void FileSystemRepository_RestoreIntegrationTest()
        {
            AppLoggingFactory.Init(new TestLoggerFactory());

            var localStorageRepository = new TestLocalStorageRepository();

            var fileSystemRepository1 = new FileSystemRepository(localStorageRepository);
            var dataFolder = fileSystemRepository1.CreateFolder(fileSystemRepository1.RootFolder.Key, "data").Result;
            fileSystemRepository1.CreateFile(dataFolder.Key, "drawr.dat").Wait();

            var fileSystemRepository2 = new FileSystemRepository(localStorageRepository);
            var items = fileSystemRepository2.GetDirectory();
        }

        private class TestLoggerFactory : ILoggerFactory
        {
            public void AddProvider(ILoggerProvider provider)
            { }

            public ILogger CreateLogger(string categoryName)
            {
                return new TestLoger();
            }

            public void Dispose()
            { }
        }

        private class TestLoger : ILogger
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                throw new NotImplementedException();
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                throw new NotImplementedException();
            }
        }

        private class TestLocalStorageRepository : ILocalStorageRepository
        {
            public IDictionary<string, string> Storage { get; } = new Dictionary<string, string>();

            public ValueTask<string> GetItem(string key)
            {
                string content;
                if (!Storage.TryGetValue(key, out content))
                    content = String.Empty;

                return new ValueTask<string>(Task.FromResult(content));
            }

            public Task<IEnumerable<string>> GetKeys()
            {
                return Task.FromResult((IEnumerable<string>)Storage.Keys);
            }

            public ValueTask RemoveItem(string key)
            {
                Storage.Remove(key);
                return new ValueTask(Task.Delay(1));
            }

            public ValueTask SetItem(string key, string content)
            {
                Storage[key] = content;
                return new ValueTask(Task.Delay(1));
            }
        }
    }
}
