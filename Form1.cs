using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Process;
using Blacklist;

namespace PadZahr
{


    public partial class MainForm : Form
    {

        public Point Mouse_Loc;

        private BackGroundProcess _backgroundProcess;
        private Button CancelButton;
        private Button DashboardButton;
        private Button SettingsButton;
        


        private Panel PanelHeader;
        private Panel PanelSidebar;
        private Panel PanelStatus;

        private Label LabelTitle;


        private Panel    PanelSettings;
        private CheckBox CheckStartup;
        private CheckBox CheckTray;

        private NotifyIcon TrayIcon;

        public const string StartupKey = "";
        //@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";


        public MainForm()
        {

            
            InitializeComponent();

            /* LOAD THE LANGUGAGES */
            LanguageManager.Load(
                Properties.Settings.Default.Language ?? "en"
            );
            /* HEADER CONTENT */

            PanelHeader = new Panel();
            PanelHeader.Height = 60;
            PanelHeader.Dock = DockStyle.Top;
            PanelHeader.BackColor = Color.FromArgb(79, 163, 227);

            LabelTitle = new Label();
            //LabelTitle.Text = LanguageManager.T("app.title");
            LabelTitle.ForeColor = Color.White;
            //LabelTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            LabelTitle.AutoSize = true;
            LabelTitle.Location = new Point(20, 17);

            PanelHeader.Controls.Add(LabelTitle);
            this.Controls.Add(PanelHeader);

            /* SIDEBAR CONTENT */
            PanelSidebar = new Panel();
            PanelSidebar.Width = 200;
            PanelSidebar.Dock = DockStyle.Left;
            PanelSidebar.BackColor = Color.White;


            Button DashboardButton = CreateMenuButton(LanguageManager.T("menu.dashboard"), 80, true);
            //CreateMenuButton("Dashboard", 80, true);
            Button SettingsButton = CreateMenuButton(LanguageManager.T("menu.settings"), 130, false);
            //ButtonScan.Text = LanguageManager.T("button.scan");
            //CreateMenuButton("Settings", 130, false);

            DashboardButton.Click += Dashboard_Click;
            SettingsButton.Click += Settings_Click;

            PanelSidebar.Controls.Add(DashboardButton);
            PanelSidebar.Controls.Add(SettingsButton);

            this.Controls.Add(PanelSidebar);

            /* Status CONTENT */
            
            PanelStatus = new Panel();
            PanelStatus.Size = new Size(650, 350);
            PanelStatus.BackColor = Color.White;
            PanelStatus.Location = new Point(80, 80);
            PanelStatus.Paint += DrawCardBorder;





            CreateSettingsPanel();

           


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
            CreateTray();


            ApplyLanguage();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close();

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

            ButtonScan.Enabled = false;
            _backgroundProcess.Start();

            if (CancelButton == null)
            {
                
                CancelButton = new Button
                {

                    Text = LanguageManager.T("button.cancel"),
                    Size = new Size(80, 25),
                    BackColor = Color.DarkRed,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    UseVisualStyleBackColor = false
                };

               
                CancelButton.FlatAppearance.BorderSize = 0;

                CancelButton.Location = new Point(
                        ButtonScan.Right + 10,
                        ButtonScan.Top
                );

                CancelButton.Click += CancelButton_Click;


                
                ButtonScan.Parent.Controls.Add(CancelButton);
            }

            CancelButton.Visible = true;
            CancelButton.BringToFront();


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

            _backgroundProcess.OnAutorunBlocked += name =>
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show(
                        $"Blocked autorun entry: {name}",
                        "PadZahr",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
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
            TrayIcon.Visible = false;
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

        private Button CreateMenuButton(string text, int top, bool selected)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 200,
                Height = 45,
                Location = new Point(0, top),
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                BackColor = selected
                    ? Color.FromArgb(79, 163, 227)
                    : Color.White,
                ForeColor = selected
                    ? Color.White
                    : Color.FromArgb(46, 46, 46)
            };

            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void DrawCardBorder(object sender, PaintEventArgs e)
        {
            Panel p = sender as Panel;
            ControlPaint.DrawBorder(
                e.Graphics,
                p.ClientRectangle,
                Color.FromArgb(229, 231, 235),
                ButtonBorderStyle.Solid
            );
        }

        private void CreateSettingsPanel()
        {
            
            PanelSettings = new Panel
            {
                Size = MainFormView.Size,
                Location = MainFormView.Location,
                BackColor = Color.White,
                Visible = false
            };

            CheckStartup = new CheckBox
            {
                Text = "Run app on startup",
                Location = new Point(20, 30),
                AutoSize = true,
                Checked = IsStartupEnabled()
            };
            CheckStartup.CheckedChanged += Startup_CheckedChanged;

            CheckTray = new CheckBox
            {
                Text = "Minimize to system tray",
                Location = new Point(20, 65),
                AutoSize = true,
                Checked = true
            };




            PanelSettings.Controls.Add(CheckStartup);
            PanelSettings.Controls.Add(CheckTray);

            // new
            ComboBox langBox = new ComboBox
            {
                Location = new Point(20, 100),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            langBox.Items.AddRange(new[] { "en", "fa" });
            // if you want add your language add new .json file and translate it based on english.json file
            // also add new string in Add Range function like "your desired language"
            langBox.SelectedItem = LanguageManager.CurrentLanguage;

            langBox.SelectedIndexChanged += (s, e) =>
            {
                string lang = langBox.SelectedItem.ToString();

                Properties.Settings.Default.Language = lang;
                Properties.Settings.Default.Save();

                Application.Restart();
            };

            PanelSettings.Controls.Add(langBox);

            Controls.Add(PanelSettings);
            PanelSettings.BringToFront();

            
        }


        private void Startup_CheckedChanged(object sender, EventArgs e)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey, true))
            {
                if (CheckStartup.Checked)
                    key.SetValue("PadZahr", Application.ExecutablePath);
                else
                    key.DeleteValue("PadZahr", false);
            }
        }

        private bool IsStartupEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupKey))
            {
                return key?.GetValue("PadZahr") != null;
            }
        }

       
        private void CreateTray()
        {
            TrayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Text = "PadZahr",
                Visible = true
            };

            TrayIcon.DoubleClick += (s, e) =>
            {
                Show();
                WindowState = FormWindowState.Normal;
            };
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            // Hide dashboard controls
            MainFormView.Visible = false;
            ButtonScan.Visible = false;
            CancelButton?.Hide();

            // Show settings controls
            PanelSettings.Visible = true;
            PanelSettings.BringToFront();
        }

        private void Dashboard_Click(object sender, EventArgs e)
        {
            // Show dashboard
            MainFormView.Visible = true;
            ButtonScan.Visible = true;

            // Hide settings
            PanelSettings.Visible = false;
        }

        private void ApplyLanguage()
        {
            ButtonScan.Text = LanguageManager.T("button.scan");
            CheckStartup.Text = LanguageManager.T("settings.startup");
            CheckTray.Text = LanguageManager.T("settings.tray");
            
            
            if (LanguageManager.CurrentLanguage == "fa")
            {
                // uncomment these two lines of code if you want to any RTL language
                // set to be on right (i wouldn't recommend it,it would break panel) 
                /*
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                */
                // Default Persian or any RTL language font
                Font persianFont = new Font("Tahoma", 9F, FontStyle.Regular);

                ApplyFontRecursive(this, persianFont);
            }
            else
            {
                // uncomment these two lines of code if you want to any RTL language
                /*
                RightToLeft = RightToLeft.No;
                RightToLeftLayout = false;
                */
                // Default English any LTR language font
                Font englishFont = new Font("Segoe UI", 9F, FontStyle.Regular);

                ApplyFontRecursive(this, englishFont);
            }
            
        }

        private void ButtonChangeToFarsi_Click(object sender, EventArgs e)
        {
            LanguageManager.Load("fa");
            ApplyLanguage();
        }

        private void ApplyFontRecursive(Control parent, Font font)
        {
            parent.Font = font;

            foreach (Control c in parent.Controls)
            {
                ApplyFontRecursive(c, font);
            }
        }
   
   
    }


    
}
