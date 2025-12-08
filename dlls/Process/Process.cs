using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Process
{
    public class Process
    {

        // Constants
        private const uint TH32CS_SNAPPROCESS = 0x00000002;

        // Struct matching PROCESSENTRY32W
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct PROCESSENTRY32 // win32 api variables for process
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        // P/Invoke declarations
        // Process modules starts here
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);
        // Process modules ends here


        // Killing modules starts here
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        private const uint PROCESS_TERMINATE = 0x0001;
        // Killing modules ends here


        // Get Process list function
       
        public static List<(string processName, uint pid)> GetProcessList()
        {
            var results = new List<(string, uint)>();

            IntPtr snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (snapshot == IntPtr.Zero || snapshot.ToInt64() == -1)
                return results;

            PROCESSENTRY32 pe32 = new PROCESSENTRY32();
            pe32.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

            if (Process32First(snapshot, ref pe32))
            {
                do
                {
                    results.Add((pe32.szExeFile, pe32.th32ProcessID));
                }
                while (Process32Next(snapshot, ref pe32));
            }

            CloseHandle(snapshot);
            return results;
        }
        
        // If program find malicous program it will kill the process
        // Killing process functions
        public static bool KillProcess(uint pid)
        {
            IntPtr handle = OpenProcess(PROCESS_TERMINATE, false, pid);
            if (handle == IntPtr.Zero)
                return false;

            bool result = TerminateProcess(handle, 0);

            CloseHandle(handle);
            return result;
        }
        /*
        // 3. Scan system → detect blacklisted process → kill them
        // -------------------------------------------------------------
        public static List<(string name, uint pid)> ScanAndTerminateBlacklisted()
        {
            List<(string name, uint pid)> killedList = new List<(string name, uint pid)>();

            var processes = ProcessApi.GetProcessList(); // your previous code

            foreach (var (name, pid) in processes)
            {
                if (Blacklist.Contains(name))
                {
                    bool killed = KillProcess(pid);

                    if (killed)
                        killedList.Add((name, pid));
                }
            }

            return killedList;
        }
        */
    }

}

