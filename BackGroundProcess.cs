// this is old implemention reading process in background
// i'll keep it here or for reading purposes
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blacklist;
using Process;


namespace PadZahr
{
    class BackGroundProcess 
    {

        private CancellationTokenSource _cts;
        private Task _workerTask;
        private readonly int _intervalMs;

        public event Action<string, uint> OnProcessKilled;

        public BackGroundProcess(int intervalMs = 5000)
        {
            _intervalMs = intervalMs;
        }

        public void Start()
        {
            if (_workerTask != null && !_workerTask.IsCompleted)
                return;

            _cts = new CancellationTokenSource();

            _workerTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                   
                    var killed = ScanAndTerminateBlacklisted();

                    foreach (var item in killed)
                    {

                        string name = item.name;
                        uint pid = item.pid;
                        
                    }

                    await Task.Delay(_intervalMs, _cts.Token);
                }
            }, _cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
        }


        // 3. Scan system continesly in background
        // -------------------------------------------------------------
        public static List<(string name, uint pid)> ScanAndTerminateBlacklisted()
        {
            List<(string name, uint pid)> killedList = new List<(string name, uint pid)>();

            var processes = Process.Process.GetProcessList();

            foreach (var (name, pid) in processes)
            {
                if (Blacklist.BlackList.Blacklist.Contains(name))
                {
                    bool killed = Process.Process.KillProcess(pid);

                    if (killed)
                        killedList.Add((name, pid));
                }
            }

            return killedList;
        }
    }

}

*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Process;
using Blacklist;
using PadZahr.Security;
using System.Diagnostics;

namespace PadZahr
{
    class BackGroundProcess
    {
        private readonly int _intervalMs;
        private CancellationTokenSource _cts;
        private Task _workerTask;

        // MalwareBazaar hash database
        private HashSet<string> _malwareHashes;

        // Autorun snapshot
        private readonly Dictionary<string, string> _knownRunEntries =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public event Action<string> OnAutorunBlocked;
        public event Action<string, uint> OnProcessKilled;

        private const string RunKeyPath =
            @"Software\Microsoft\Windows\CurrentVersion\Run";

        public BackGroundProcess(int intervalMs = 3000)
        {
            _intervalMs = intervalMs;

            // Load malware hashes from malwarebazar website
            _malwareHashes = MalwareBazaarUpdater.LoadHashes();

            SnapshotRunKeys();
        }

        public void Start()
        {
            if (_workerTask != null && !_workerTask.IsCompleted)
                return;

            _cts = new CancellationTokenSource();

            _workerTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    ScanProcesses();
                    ScanRunKeysOnce();

                    await Task.Delay(_intervalMs, _cts.Token);
                }
            }, _cts.Token);
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        /*
         * Process scanning ( name + hash we provided )
        */ 
        private void ScanProcesses()
        {
            foreach (var proc in System.Diagnostics.Process.GetProcesses())
            {
                try
                {
                    string processName = proc.ProcessName + ".exe";
                    uint pid = (uint)proc.Id;

                    // first Name-based blacklist (scans based on my internal blacklist.dll)
                    if (BlackList.Blacklist.Contains(processName))
                    {
                        KillProcess(proc, processName, pid);
                        continue;
                    }

                    // second Hash-based detection (MalwareBazaar)
                    string hash = HashChecker.GetProcessHash(proc);
                    if (hash != null && _malwareHashes.Contains(hash))
                    {
                        KillProcess(proc, processName, pid);
                    }
                }
                catch
                {
                   
                }
            }
        }

        private void KillProcess(System.Diagnostics.Process proc, string name, uint pid)
        {
            try
            {
                proc.Kill();
                OnProcessKilled?.Invoke(name, pid);
            }
            catch { }
        }

        /* 
        * Autorun protection
        */
        private void SnapshotRunKeys()
        {
            var key = Registry.CurrentUser.OpenSubKey(RunKeyPath);
            if (key == null) return;

            foreach (var name in key.GetValueNames())
            {
                _knownRunEntries[name] = key.GetValue(name)?.ToString();
            }
        }

        private void ScanRunKeysOnce()
        {
            var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
            if (key == null) return;

            foreach (var name in key.GetValueNames())
            {
                if (_knownRunEntries.ContainsKey(name))
                    continue;

                string value = key.GetValue(name)?.ToString();
                _knownRunEntries[name] = value;

                TryKillProcessFromPath(value);

                key.DeleteValue(name, false);
                OnAutorunBlocked?.Invoke(name);
            }
        }

        private void TryKillProcessFromPath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            string exePath = value.Trim('"');

            foreach (var proc in System.Diagnostics.Process.GetProcesses())
            {
                try
                {
                    if (proc.MainModule.FileName.Equals(exePath, StringComparison.OrdinalIgnoreCase))
                    {
                        KillProcess(proc, proc.ProcessName + ".exe", (uint)proc.Id);
                    }
                }
                catch { }
            }
        }
    }
}
