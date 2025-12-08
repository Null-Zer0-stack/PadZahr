using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Process;
using Blacklist;

namespace PadZahr
{
    public partial class MainForm : Form
    {
        public MainForm()
        {

            this.BackColor = ColorTranslator.FromHtml("#111827");
            InitializeComponent();
            var list_process = Process.Process.GetProcessList();
            MainFormView.View = View.Details;
            MainFormView.Columns.Add("Process Name", 200);
            MainFormView.Columns.Add("PID", 80);
            LoadProcesses();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();
            // CloseButton.FlatStyle = FlatStyle.Flat;
            // CloseButton.FlatAppearance.BorderSize = 0;
            //CloseButton.BackColor = Color.DarkRed;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void LoadProcesses()
        {
            MainFormView.Items.Clear();

            var processes = Process.Process.GetProcessList();

            foreach (var (processName, pid) in processes)
            {
                var item = new ListViewItem(processName);
                item.SubItems.Add(pid.ToString());
                MainFormView.Items.Add(item);
            }
        }

        public static List<(string name, uint pid)> ScanAndTerminateBlacklisted()
        {

            List<(string name, uint pid)> killedList = new List<(string name, uint pid)>();

            var processes = Process.Process.GetProcessList(); // your previous code

            foreach (var (name, pid) in processes)
            {
                
                if (BlackList.Blacklist.Contains(name))
                {
                    
                    bool killed = Process.Process.KillProcess(pid);

                    if (killed)
                        killedList.Add((name, pid));
            
                }
            }

            return killedList;
        }

        private void ButtonScan_Click(object sender, EventArgs e)
        {
            var killed = ScanAndTerminateBlacklisted();

            MainFormView.Items.Clear();
            foreach (var (name, pid) in killed)
            {
                var item = new ListViewItem(name);
                item.SubItems.Add(pid.ToString());
                MainFormView.Items.Add(item);
            }
        }
    }
}
