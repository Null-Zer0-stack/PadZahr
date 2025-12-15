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
        private BackGroundProcess _backgroundProcess;
        private Button CancelButton;
        public Point Mouse_Loc;

        public MainForm()
        {

            this.BackColor = ColorTranslator.FromHtml("#111827");
            InitializeComponent();
            var list_process = Process.Process.GetProcessList();
            MainFormView.View = View.Details;
            MainFormView.Columns.Add("Process Name", 200);
            MainFormView.Columns.Add("PID", 80);

            _backgroundProcess = new BackGroundProcess(3000);

            _backgroundProcess.OnProcessKilled += (name, pid) =>
            {
                Invoke(new Action(() =>
                {
                    var item = new ListViewItem(name);
                    item.SubItems.Add(pid.ToString());
                    MainFormView.Items.Add(item);
                }));
            };

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

        private void ButtonScan_Click(object sender, EventArgs e)
        {

            ButtonScan.Enabled = false; // prevent multiple starts
            _backgroundProcess.Start();



            if (CancelButton == null)
            {
                CancelButton = new Button();
                CancelButton.Text = "Cancel";
                CancelButton.Size = new Size(80, 25);

                CancelButton.BackColor = Color.DarkRed;
                CancelButton.ForeColor = Color.White;
                CancelButton.UseVisualStyleBackColor = false;

                CancelButton.FlatStyle = FlatStyle.Flat;
                CancelButton.FlatAppearance.BorderSize = 0;

                CancelButton.Location = new Point(
                ButtonScan.Location.X + ButtonScan.Width + 10,
                ButtonScan.Location.Y
                );

                CancelButton.Click += CancelButton_Click;

                this.Controls.Add(CancelButton);
            }

            CancelButton.Visible = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _backgroundProcess = new BackGroundProcess(3000);

            _backgroundProcess.OnProcessKilled += (name, pid) =>
            {
                Invoke(new Action(() =>
                {
                    var item = new ListViewItem(name);
                    item.SubItems.Add(pid.ToString());
                    MainFormView.Items.Add(item);
                }));
            };
        }


        private void CancelButton_Click(object sender, EventArgs e)
        {
            _backgroundProcess.Stop();

            ButtonScan.Enabled = true;
            CancelButton.Visible = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _backgroundProcess?.Stop();
        }


        private void Mouse_Down(object sender, MouseEventArgs e)
        {
            Mouse_Loc = new Point(-e.X, -e.Y);
        }

        private void Moveable(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                Point Mouse_Poistion = Control.MousePosition;
                Mouse_Poistion.Offset(Mouse_Loc.X, Mouse_Loc.Y);
                Location = Mouse_Poistion;

            }

        }
    }
}
