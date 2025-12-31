using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PadZahr.Security
{
    public class ScanProgress
    {
        public int TotalFiles { get; set; }
        public int ScannedFiles { get; set; }
        public string CurrentFile { get; set; }

        public int Percent =>
            TotalFiles == 0 ? 0 :
            (ScannedFiles * 100) / TotalFiles;
    }
}