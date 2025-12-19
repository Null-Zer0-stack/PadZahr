
namespace PadZahr
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.CloseButton = new System.Windows.Forms.Button();
            this.MinimizeButton = new System.Windows.Forms.Button();
            this.MainFormView = new System.Windows.Forms.ListView();
            this.ButtonScan = new System.Windows.Forms.Button();
            this.PadzahrLabel = new System.Windows.Forms.Label();
            this.PadZahrPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PadZahrPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.LightCoral;
            this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CloseButton.Location = new System.Drawing.Point(819, 1);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(55, 23);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "X";
            this.CloseButton.UseVisualStyleBackColor = false;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.MinimizeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MinimizeButton.Location = new System.Drawing.Point(758, 1);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Size = new System.Drawing.Size(55, 23);
            this.MinimizeButton.TabIndex = 1;
            this.MinimizeButton.Text = "-";
            this.MinimizeButton.UseVisualStyleBackColor = false;
            this.MinimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            // 
            // MainFormView
            // 
            this.MainFormView.HideSelection = false;
            this.MainFormView.Location = new System.Drawing.Point(251, 70);
            this.MainFormView.Name = "MainFormView";
            this.MainFormView.Size = new System.Drawing.Size(612, 382);
            this.MainFormView.TabIndex = 2;
            this.MainFormView.UseCompatibleStateImageBehavior = false;
            // 
            // ButtonScan
            // 
            this.ButtonScan.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonScan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonScan.Location = new System.Drawing.Point(251, 32);
            this.ButtonScan.Name = "ButtonScan";
            this.ButtonScan.Size = new System.Drawing.Size(75, 23);
            this.ButtonScan.TabIndex = 3;
            this.ButtonScan.Text = "Scan";
            this.ButtonScan.UseVisualStyleBackColor = false;
            this.ButtonScan.Click += new System.EventHandler(this.ButtonScan_Click);
            // 
            // PadzahrLabel
            // 
            this.PadzahrLabel.AutoSize = true;
            this.PadzahrLabel.BackColor = System.Drawing.Color.White;
            this.PadzahrLabel.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PadzahrLabel.Location = new System.Drawing.Point(97, 24);
            this.PadzahrLabel.Name = "PadzahrLabel";
            this.PadzahrLabel.Size = new System.Drawing.Size(89, 30);
            this.PadzahrLabel.TabIndex = 4;
            this.PadzahrLabel.Text = "PadZahr";
            // 
            // PadZahrPictureBox
            // 
            this.PadZahrPictureBox.BackColor = System.Drawing.Color.White;
            this.PadZahrPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("PadZahrPictureBox.Image")));
            this.PadZahrPictureBox.Location = new System.Drawing.Point(23, 12);
            this.PadZahrPictureBox.Name = "PadZahrPictureBox";
            this.PadZahrPictureBox.Size = new System.Drawing.Size(53, 53);
            this.PadZahrPictureBox.TabIndex = 5;
            this.PadZahrPictureBox.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(875, 464);
            this.Controls.Add(this.PadZahrPictureBox);
            this.Controls.Add(this.PadzahrLabel);
            this.Controls.Add(this.ButtonScan);
            this.Controls.Add(this.MainFormView);
            this.Controls.Add(this.MinimizeButton);
            this.Controls.Add(this.CloseButton);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PadZahr(Real Time Anti-virus)";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Mouse_Down);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Moveable);
            ((System.ComponentModel.ISupportInitialize)(this.PadZahrPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button MinimizeButton;
        private System.Windows.Forms.ListView MainFormView;
        private System.Windows.Forms.Button ButtonScan;
        private System.Windows.Forms.Label PadzahrLabel;
        private System.Windows.Forms.PictureBox PadZahrPictureBox;
    }
}

