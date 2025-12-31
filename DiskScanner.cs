using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace PadZahr.Security
{
    public static class DiskScanner
    {
        private static readonly string[] TargetExtensions =
        {
            ".exe", ".dll", ".scr"
        };

        public static List<ScanResult> ScanFolder(
            string root,
            HashSet<string> malwareHashes,
            Action<ScanProgress> onProgress)
        {
            var results = new List<ScanResult>();
            var files = new List<string>();

            CollectFiles(root, files);

            var progress = new ScanProgress
            {
                TotalFiles = files.Count
            };

            foreach (string file in files)
            {
                progress.ScannedFiles++;
                progress.CurrentFile = file;
                onProgress?.Invoke(progress);

                try
                {
                    FileInfo info = new FileInfo(file);
                    bool hidden = info.Attributes.HasFlag(FileAttributes.Hidden);

                    string hash = HashChecker.GetFileHash(file);
                    bool isMalware =
                        !string.IsNullOrEmpty(hash) &&
                        malwareHashes.Contains(hash);

                    if (hidden || isMalware)
                    {
                        results.Add(new ScanResult
                        {
                            FilePath = file,
                            Hash = hash,
                            IsHidden = hidden,
                            IsMalware = isMalware,
                            Reason = isMalware
                                ? "MalwareBazaar hash match"
                                : "Hidden executable"
                        });
                    }
                }
                catch
                {
                    
                }
            }

            return results;
        }

        private static void CollectFiles(string dir, List<string> files)
        {
            try
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    string ext = Path.GetExtension(file).ToLowerInvariant();
                    if (Array.IndexOf(TargetExtensions, ext) != -1)
                        files.Add(file);
                }

                foreach (string sub in Directory.GetDirectories(dir))
                {
                    CollectFiles(sub, files);
                }
            }
            catch
            {
               
            }
        }
    }
}