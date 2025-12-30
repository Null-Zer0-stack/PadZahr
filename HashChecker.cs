using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Process;

namespace PadZahr.Security
{
    public static class HashChecker
    {
        public static string ComputeSHA256(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public static string GetProcessHash(System.Diagnostics.Process process)
        {
            try
            {
                return ComputeSHA256(process.MainModule.FileName);
            }
            catch
            {
                return null; // Access denied/system process
            }
        }
    }
}
