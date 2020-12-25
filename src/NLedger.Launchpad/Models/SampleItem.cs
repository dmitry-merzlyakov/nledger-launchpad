using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NLedger.Launchpad.Models
{
    public class SampleItem
    {
        public Guid SampleID { get; set; }
        public string Title { get; set; }
        public string Command { get; set; }
        public List<string> Files { get; set; }
    }

    public class SampleFile
    {
        public string Content { get; set; }
    }
}
