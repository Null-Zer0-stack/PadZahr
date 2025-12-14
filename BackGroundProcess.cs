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
using Blacklist;
using Process;

namespace PadZahr
{
    class BackGroundProcess
    {
        private CancellationTokenSource _cts;
        private Task _workerTask;
        private readonly int _intervalMs;

        // MEMORY: processes that existed before scan started
        private readonly HashSet<uint> _knownPids = new HashSet<uint>();

        public event Action<string, uint> OnProcessKilled;

        public BackGroundProcess(int intervalMs = 5000)
        {
            _intervalMs = intervalMs;

            // Snapshot existing processes ONCE
            foreach (var (_, pid) in Process.Process.GetProcessList())
            {
                _knownPids.Add(pid);
            }
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
                    ScanOnce();
                    await Task.Delay(_intervalMs, _cts.Token);
                }
            });
        }

        public void Stop()
        {
            _cts?.Cancel();
        }

        private void ScanOnce()
        {
            var processes = Process.Process.GetProcessList();

            foreach (var (name, pid) in processes)
            {
                // Skip processes we already know
                if (_knownPids.Contains(pid))
                    continue;

                _knownPids.Add(pid);

                if (BlackList.Blacklist.Contains(name))
                {
                    if (Process.Process.KillProcess(pid))
                    {
                        OnProcessKilled?.Invoke(name, pid);
                    }
                }
            }
        }
    }
}