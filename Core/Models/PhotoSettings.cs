using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Vega.Core.Models
{
    public class PhotoSettings
    {
        public int MinBytes { get; set; }
        public int MaxBytes { get; set; }
        public string[] AcceptedFileTypes { get; set; }

        public bool HasValidExtension(string fileName)
        {
            return AcceptedFileTypes.Any(x => x == Path.GetExtension(fileName).ToLower());
        }
    }
}
