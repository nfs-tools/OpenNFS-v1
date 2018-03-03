namespace OpenNFSUI
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.exitButton = new DarkUI.Controls.DarkButton();
            this.saveButton = new DarkUI.Controls.DarkButton();
            this.appPanel = new System.Windows.Forms.Panel();
            this.arrayPanel = new System.Windows.Forms.Panel();
            this.panelArr1 = new System.Windows.Forms.Panel();
            this.panelArr8 = new System.Windows.Forms.Panel();
            this.panelArr2 = new System.Windows.Forms.Panel();
            this.panelArr7 = new System.Windows.Forms.Panel();
            this.panelArr3 = new System.Windows.Forms.Panel();
            this.panelArr6 = new System.Windows.Forms.Panel();
            this.panelArr4 = new System.Windows.Forms.Panel();
            this.panelArr5 = new System.Windows.Forms.Panel();
            this.gameDirStatusLabel = new DarkUI.Controls.DarkLabel();
            this.startupUpdateStatusLabel = new DarkUI.Controls.DarkLabel();
            this.upInfTitleLabel = new DarkUI.Controls.DarkLabel();
            this.checkUpdatesCB = new DarkUI.Controls.DarkCheckBox();
            this.darkLabel3 = new DarkUI.Controls.DarkLabel();
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.appLabel = new DarkUI.Controls.DarkLabel();
            this.appPanel.SuspendLayout();
            this.arrayPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.Location = new System.Drawing.Point(537, 406);
            this.exitButton.Name = "exitButton";
            this.exitButton.Padding = new System.Windows.Forms.Padding(5);
            this.exitButton.Size = new System.Drawing.Size(75, 23);
            this.exitButton.TabIndex = 1;
            this.exitButton.Text = "Exit";
            this.exitButton.Click += new System.EventHandler(this.exitButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(456, 406);
            this.saveButton.Name = "saveButton";
            this.saveButton.Padding = new System.Windows.Forms.Padding(5);
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            // 
            // appPanel
            // 
            this.appPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appPanel.AutoScroll = true;
            this.appPanel.Controls.Add(this.arrayPanel);
            this.appPanel.Controls.Add(this.gameDirStatusLabel);
            this.appPanel.Controls.Add(this.startupUpdateStatusLabel);
            this.appPanel.Controls.Add(this.upInfTitleLabel);
            this.appPanel.Controls.Add(this.checkUpdatesCB);
            this.appPanel.Controls.Add(this.darkLabel3);
            this.appPanel.Controls.Add(this.darkLabel1);
            this.appPanel.Controls.Add(this.darkLabel2);
            this.appPanel.Controls.Add(this.appLabel);
            this.appPanel.Location = new System.Drawing.Point(12, 12);
            this.appPanel.Name = "appPanel";
            this.appPanel.Size = new System.Drawing.Size(600, 388);
            this.appPanel.TabIndex = 3;
            this.appPanel.MouseEnter += new System.EventHandler(this.appPanel_MouseEnter);
            // 
            // arrayPanel
            // 
            this.arrayPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.arrayPanel.Controls.Add(this.panelArr1);
            this.arrayPanel.Controls.Add(this.panelArr8);
            this.arrayPanel.Controls.Add(this.panelArr2);
            this.arrayPanel.Controls.Add(this.panelArr7);
            this.arrayPanel.Controls.Add(this.panelArr3);
            this.arrayPanel.Controls.Add(this.panelArr6);
            this.arrayPanel.Controls.Add(this.panelArr4);
            this.arrayPanel.Controls.Add(this.panelArr5);
            this.arrayPanel.Location = new System.Drawing.Point(7, 173);
            this.arrayPanel.Name = "arrayPanel";
            this.arrayPanel.Size = new System.Drawing.Size(587, 49);
            this.arrayPanel.TabIndex = 7;
            // 
            // panelArr1
            // 
            this.panelArr1.Location = new System.Drawing.Point(11, 9);
            this.panelArr1.Name = "panelArr1";
            this.panelArr1.Size = new System.Drawing.Size(32, 32);
            this.panelArr1.TabIndex = 6;
            // 
            // panelArr8
            // 
            this.panelArr8.Location = new System.Drawing.Point(277, 9);
            this.panelArr8.Name = "panelArr8";
            this.panelArr8.Size = new System.Drawing.Size(32, 32);
            this.panelArr8.TabIndex = 6;
            // 
            // panelArr2
            // 
            this.panelArr2.Location = new System.Drawing.Point(49, 9);
            this.panelArr2.Name = "panelArr2";
            this.panelArr2.Size = new System.Drawing.Size(32, 32);
            this.panelArr2.TabIndex = 6;
            // 
            // panelArr7
            // 
            this.panelArr7.Location = new System.Drawing.Point(239, 9);
            this.panelArr7.Name = "panelArr7";
            this.panelArr7.Size = new System.Drawing.Size(32, 32);
            this.panelArr7.TabIndex = 6;
            // 
            // panelArr3
            // 
            this.panelArr3.Location = new System.Drawing.Point(87, 9);
            this.panelArr3.Name = "panelArr3";
            this.panelArr3.Size = new System.Drawing.Size(32, 32);
            this.panelArr3.TabIndex = 6;
            // 
            // panelArr6
            // 
            this.panelArr6.Location = new System.Drawing.Point(201, 9);
            this.panelArr6.Name = "panelArr6";
            this.panelArr6.Size = new System.Drawing.Size(32, 32);
            this.panelArr6.TabIndex = 6;
            // 
            // panelArr4
            // 
            this.panelArr4.Location = new System.Drawing.Point(125, 9);
            this.panelArr4.Name = "panelArr4";
            this.panelArr4.Size = new System.Drawing.Size(32, 32);
            this.panelArr4.TabIndex = 6;
            // 
            // panelArr5
            // 
            this.panelArr5.Location = new System.Drawing.Point(163, 9);
            this.panelArr5.Name = "panelArr5";
            this.panelArr5.Size = new System.Drawing.Size(32, 32);
            this.panelArr5.TabIndex = 6;
            // 
            // gameDirStatusLabel
            // 
            this.gameDirStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameDirStatusLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gameDirStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.gameDirStatusLabel.Location = new System.Drawing.Point(16, 225);
            this.gameDirStatusLabel.Name = "gameDirStatusLabel";
            this.gameDirStatusLabel.Size = new System.Drawing.Size(568, 50);
            this.gameDirStatusLabel.TabIndex = 5;
            this.gameDirStatusLabel.Text = "Click on a game to select it\'s directory.";
            // 
            // startupUpdateStatusLabel
            // 
            this.startupUpdateStatusLabel.AutoSize = true;
            this.startupUpdateStatusLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startupUpdateStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.startupUpdateStatusLabel.Location = new System.Drawing.Point(16, 66);
            this.startupUpdateStatusLabel.Name = "startupUpdateStatusLabel";
            this.startupUpdateStatusLabel.Size = new System.Drawing.Size(306, 17);
            this.startupUpdateStatusLabel.TabIndex = 5;
            this.startupUpdateStatusLabel.Text = "The application will check for updates on startup.";
            // 
            // upInfTitleLabel
            // 
            this.upInfTitleLabel.AutoSize = true;
            this.upInfTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.upInfTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.upInfTitleLabel.Location = new System.Drawing.Point(16, 46);
            this.upInfTitleLabel.Name = "upInfTitleLabel";
            this.upInfTitleLabel.Size = new System.Drawing.Size(58, 17);
            this.upInfTitleLabel.TabIndex = 5;
            this.upInfTitleLabel.Text = "Updates";
            // 
            // checkUpdatesCB
            // 
            this.checkUpdatesCB.AutoSize = true;
            this.checkUpdatesCB.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkUpdatesCB.Location = new System.Drawing.Point(19, 98);
            this.checkUpdatesCB.Name = "checkUpdatesCB";
            this.checkUpdatesCB.Size = new System.Drawing.Size(117, 19);
            this.checkUpdatesCB.TabIndex = 4;
            this.checkUpdatesCB.Text = "Check on Startup";
            this.checkUpdatesCB.CheckedChanged += new System.EventHandler(this.checkUpdatesCB_CheckedChanged);
            // 
            // darkLabel3
            // 
            this.darkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.darkLabel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.darkLabel3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkLabel3.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel3.Location = new System.Drawing.Point(3, 168);
            this.darkLabel3.Name = "darkLabel3";
            this.darkLabel3.Size = new System.Drawing.Size(591, 2);
            this.darkLabel3.TabIndex = 2;
            this.darkLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // darkLabel1
            // 
            this.darkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.darkLabel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.darkLabel1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkLabel1.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel1.Location = new System.Drawing.Point(3, 35);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(591, 2);
            this.darkLabel1.TabIndex = 2;
            this.darkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // darkLabel2
            // 
            this.darkLabel2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.darkLabel2.ForeColor = System.Drawing.Color.DarkGray;
            this.darkLabel2.Location = new System.Drawing.Point(3, 142);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(509, 25);
            this.darkLabel2.TabIndex = 3;
            this.darkLabel2.Text = "Game Directories";
            this.darkLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // appLabel
            // 
            this.appLabel.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appLabel.ForeColor = System.Drawing.Color.DarkGray;
            this.appLabel.Location = new System.Drawing.Point(3, 9);
            this.appLabel.Name = "appLabel";
            this.appLabel.Size = new System.Drawing.Size(509, 25);
            this.appLabel.TabIndex = 3;
            this.appLabel.Text = "Application";
            this.appLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.appPanel);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.exitButton);
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "SettingsForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.appPanel.ResumeLayout(false);
            this.appPanel.PerformLayout();
            this.arrayPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private DarkUI.Controls.DarkButton exitButton;
        private DarkUI.Controls.DarkButton saveButton;
        private System.Windows.Forms.Panel appPanel;
        private DarkUI.Controls.DarkCheckBox checkUpdatesCB;
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkLabel appLabel;
        private DarkUI.Controls.DarkLabel upInfTitleLabel;
        private DarkUI.Controls.DarkLabel startupUpdateStatusLabel;
        private DarkUI.Controls.DarkLabel darkLabel3;
        private DarkUI.Controls.DarkLabel darkLabel2;
        private DarkUI.Controls.DarkLabel gameDirStatusLabel;
        private System.Windows.Forms.Panel panelArr8;
        private System.Windows.Forms.Panel panelArr7;
        private System.Windows.Forms.Panel panelArr6;
        private System.Windows.Forms.Panel panelArr5;
        private System.Windows.Forms.Panel panelArr4;
        private System.Windows.Forms.Panel panelArr3;
        private System.Windows.Forms.Panel panelArr2;
        private System.Windows.Forms.Panel panelArr1;
        private System.Windows.Forms.Panel arrayPanel;
    }
}