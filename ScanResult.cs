using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadZahr.Security
{
    public class ScanResult
    {
        public string FilePath { get; set; }
        public string Hash { get; set; }
        public bool IsHidden { get; set; }
        public bool IsMalware { get; set; }
        public string Reason { get; set; }
    }
}