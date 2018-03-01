namespace OpenNFSUI.Docking
{
    partial class DockExplorer
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
            this.explorerTreeView = new DarkUI.Controls.DarkTreeView();
            this.SuspendLayout();
            // 
            // explorerTreeView
            // 
            this.explorerTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerTreeView.Location = new System.Drawing.Point(0, 25);
            this.explorerTreeView.MaxDragChange = 20;
            this.explorerTreeView.Name = "explorerTreeView";
            this.explorerTreeView.Size = new System.Drawing.Size(327, 486);
            this.explorerTreeView.TabIndex = 0;
            this.explorerTreeView.Text = "Explorer Tree View";
            // 
            // DockExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.explorerTreeView);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Left;
            this.DockText = "Explorer";
            this.Icon = global::OpenNFSUI.Properties.Resources.application_16x;
            this.Name = "DockExplorer";
            this.Size = new System.Drawing.Size(327, 511);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkTreeView explorerTreeView;
    }
}
