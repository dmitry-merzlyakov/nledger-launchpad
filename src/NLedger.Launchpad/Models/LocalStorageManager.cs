using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class LocalStorageManager
    {
        public LocalStorageManager(ILocalStorageRepository localStorageRepository)
        {
            if (localStorageRepository == null)
                throw new ArgumentNullException(nameof(localStorageRepository));

            LocalStorageRepository = localStorageRepository;
        }


        public ILocalStorageRepository LocalStorageRepository { get; }

        public async Task Clear()
        {
            foreach(var file in (await LocalStorageRepository.GetKeys()).ToArray())
            {
                await LocalStorageRepository.RemoveItem(file);
            }
        }

        public async Task<byte[]> ExportToZip()
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var files = (await LocalStorageRepository.GetKeys()).OrderBy(f => f).ToArray();

                    using (var hash = new MD5CompositeHash())
                    {

                        foreach (var file in files)
                        {
                            var entry = archive.CreateEntry(file);

                            using (var entryStream = entry.Open())
                            {
                                using (var streamWriter = new StreamWriter(entryStream))
                                {
                                    var content = await LocalStorageRepository.GetItem(file);
                                    streamWriter.Write(content);

                                    hash.Add(file);
                                    hash.Add(content);
                                }
                            }
                        }

                        var hashEntry = archive.CreateEntry(SignatureName);
                        using (var entryStream = hashEntry.Open())
                        {
                            using (var streamWriter = new StreamWriter(entryStream))
                            {
                                streamWriter.Write(hash.Hash.ToString());
                            }
                        }

                    }
                }

                return memoryStream.ToArray();
            }
        }

        public async Task ImportFromZip(byte[] zip)
        {
            if (zip == null)
                throw new ArgumentNullException(nameof(zip));

            if ((await LocalStorageRepository.GetKeys()).Any())
                throw new InvalidOperationException("Local storage must be empty before import");

            using (var memoryStream = new MemoryStream(zip))
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
                {
                    foreach(var entry in archive.Entries)
                    {
                        if (entry.Name != SignatureName)
                        {
                            using (var entryStream = entry.Open())
                            {
                                using (var streamWriter = new StreamReader(entryStream))
                                {
                                    await LocalStorageRepository.SetItem(entry.Name, streamWriter.ReadToEnd());
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool ValidateZip(byte[] zip)
        {
            if (zip == null)
                throw new ArgumentNullException(nameof(zip));

            var signature = String.Empty;
            using (var hash = new MD5CompositeHash())
            {
                using (var memoryStream = new MemoryStream(zip))
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read, true))
                    {
                        foreach (var entry in archive.Entries.OrderBy(n => n.Name))
                        {
                            using (var entryStream = entry.Open())
                            {
                                using (var streamWriter = new StreamReader(entryStream))
                                {
                                    var content = streamWriter.ReadToEnd();

                                    if (entry.Name == SignatureName)
                                        signature = content;
                                    else
                                    {
                                        hash.Add(entry.Name);
                                        hash.Add(content);
                                    }
                                }
                            }
                        }
                    }
                }

                return signature == hash.Hash.ToString();
            }
        }

        private static readonly string SignatureName = "signature";

        private class MD5CompositeHash : IDisposable
        {
            public MD5CompositeHash()
            {
                MD5 = MD5.Create();
            }

            public MD5 MD5 { get; }

            public Guid Hash { get; private set; }

            public void Add(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return;

                var inputBytes = Encoding.UTF8.GetBytes(text);
                var hashBytes = MD5.ComputeHash(inputBytes);
                var hashGuid = new Guid(hashBytes);

                Hash = Hash == default(Guid) ? hashGuid : Xor(Hash, hashGuid);
            }

            public void Dispose()
            {
                MD5.Dispose();
            }

            private Guid Xor(Guid g1, Guid g2)
            {
                var ba1 = g1.ToByteArray();
                var ba2 = g2.ToByteArray();
                var ba3 = new byte[16];

                for (int i = 0; i < 16; i++)
                {
                    ba3[i] = (byte)(ba1[i] ^ ba2[i]);
                }

                return new Guid(ba3);
            }
        }

    }
}
