using NLedger.Launchpad.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Repositories.FileSystem
{
    public class FileSystemInputStream : FileSystemItem
    {
        public static readonly Guid InputStreamKey = new Guid("{CDAF4F5D-8D04-48E5-B123-21B0FD91AAA8}");
        public static readonly string InputStreamName = "[Input Stream]";

        public FileSystemInputStream()
            : base(InputStreamKey, InputStreamName)
        { }

        public override FileSystemItemKind Kind => FileSystemItemKind.InputStream;
    }
}
