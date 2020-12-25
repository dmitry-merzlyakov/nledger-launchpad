using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public class FileSystemItemDto
    {
        public Guid Key { get; set; }
        public Guid ParentKey { get; set; }
        public string Name { get; set; }
        public FileSystemItemKind Kind { get; set; }
    }

    public class FileSystemItemDtoContainer
    {
        public IList<FileSystemItemDto> Items { get; set; }
    }

    public class FileSystemDirectorySerializer
    {
        public string Serialize(IEnumerable<FileSystemItem> fileSystemItems)
        {
            if (fileSystemItems == null)
                throw new ArgumentNullException(nameof(fileSystemItems));

            var items = fileSystemItems.Select(f => new FileSystemItemDto()
            {
                Key = f.Key,
                ParentKey = f.ParentFolder?.Key ?? default(Guid),
                Name = f.Name,
                Kind = f.Kind
            }).ToList();

            return System.Text.Json.JsonSerializer.Serialize(new FileSystemItemDtoContainer() { Items = items });
        }

        public IEnumerable<FileSystemItemDto> Deserialize(string serializedItems)
        {
            if (String.IsNullOrWhiteSpace(serializedItems))
                return Enumerable.Empty<FileSystemItemDto>();

            var container = System.Text.Json.JsonSerializer.Deserialize<FileSystemItemDtoContainer>(serializedItems);
            return (container?.Items) ?? Enumerable.Empty<FileSystemItemDto>();
        }
    }
}
