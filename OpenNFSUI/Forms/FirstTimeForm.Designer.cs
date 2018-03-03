namespace OpenNFSUI
{
    partial class FirstTimeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirstTimeForm));
            this.welcomeLabel = new DarkUI.Controls.DarkLabel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.continueButton = new DarkUI.Controls.DarkButton();
            this.logoPB = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.logoPB)).BeginInit();
            this.SuspendLayout();
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.welcomeLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.welcomeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.welcomeLabel.Location = new System.Drawing.Point(12, 125);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(541, 52);
            this.welcomeLabel.TabIndex = 2;
            this.welcomeLabel.Text = "First Time Setup";
            this.welcomeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelButtons
            // 
            this.panelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelButtons.AutoScroll = true;
            this.panelButtons.Location = new System.Drawing.Point(12, 180);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(541, 355);
            this.panelButtons.TabIndex = 3;
            // 
            // continueButton
            // 
            this.continueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.continueButton.Enabled = false;
            this.continueButton.Location = new System.Drawing.Point(467, 541);
            this.continueButton.Name = "continueButton";
            this.continueButton.Padding = new System.Windows.Forms.Padding(5);
            this.continueButton.Size = new System.Drawing.Size(86, 23);
            this.continueButton.TabIndex = 4;
            this.continueButton.Text = "Continue";
            // 
            // logoPB
            // 
            this.logoPB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logoPB.BackgroundImage = global::OpenNFSUI.Properties.Resources.opennfs_logo_shadowless;
            this.logoPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.logoPB.Location = new System.Drawing.Point(12, 12);
            this.logoPB.Name = "logoPB";
            this.logoPB.Size = new System.Drawing.Size(541, 110);
            this.logoPB.TabIndex = 1;
            this.logoPB.TabStop = false;
            // 
            // FirstTimeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ClientSize = new System.Drawing.Size(565, 572);
            this.Controls.Add(this.continueButton);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.welcomeLabel);
            this.Controls.Add(this.logoPB);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(581, 611);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(581, 611);
            this.Name = "FirstTimeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "OpenNFS - First Time Setup";
            this.Load += new System.EventHandler(this.FirstTimeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.logoPB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox logoPB;
        private DarkUI.Controls.DarkLabel welcomeLabel;
        private System.Windows.Forms.Panel panelButtons;
        private DarkUI.Controls.DarkButton continueButton;
    }
}