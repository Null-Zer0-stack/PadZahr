// this is old implemention reading process in background
// i'll keep it here or reading purposes
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Blacklist;
using Process;

namespace PadZahr
{
    class BackGroundProcess
    {
        private readonly int _intervalMs;
        private CancellationTokenSource _cts;
        private Task _workerTask;

        // Snapshot of autorun values
        private readonly Dictionary<string, string> _knownRunEntries =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


        public event Action<string> OnAutorunBlocked;
        public event Action<string, uint> OnProcessKilled;

        private const string RunKeyPath =
            @"Software\Microsoft\Windows\CurrentVersion\Run";

        public BackGroundProcess(int intervalMs = 3000)
        {
            _intervalMs = intervalMs;
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
                    ScanRunKeysOnce();
                    await Task.Delay(_intervalMs, _cts.Token);
                }
            });
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        // -----------------------------
        // Registry logic
        // -----------------------------

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

                // Try to kill the process
                TryKillProcessFromPath(value);

                // Remove autorun
                key.DeleteValue(name, false);

                OnAutorunBlocked?.Invoke(name);
            }
        }

        private void TryKillProcessFromPath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            string exePath = value.Trim('"');

            foreach (var (procName, pid) in Process.Process.GetProcessList())
            {
                try
                {
                    if (exePath.EndsWith(procName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (Process.Process.KillProcess(pid))
                        {
                            OnProcessKilled?.Invoke(procName, pid);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
