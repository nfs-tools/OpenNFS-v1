namespace OpenNFSUI.Controls
{
    partial class SettingsGameIconControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.iconPB = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.iconPB)).BeginInit();
            this.SuspendLayout();
            // 
            // iconPB
            // 
            this.iconPB.BackgroundImage = global::OpenNFSUI.Properties.Resources.application_16x;
            this.iconPB.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.iconPB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.iconPB.Location = new System.Drawing.Point(0, 0);
            this.iconPB.Name = "iconPB";
            this.iconPB.Size = new System.Drawing.Size(32, 32);
            this.iconPB.TabIndex = 0;
            this.iconPB.TabStop = false;
            // 
            // SettingsGameIconControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.iconPB);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Name = "SettingsGameIconControl";
            this.Size = new System.Drawing.Size(32, 32);
            this.Load += new System.EventHandler(this.SettingsGameIconControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.iconPB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox iconPB;
    }
}
