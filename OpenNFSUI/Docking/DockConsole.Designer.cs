namespace OpenNFSUI.Docking
{
    partial class DockConsole
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
            this.consoleListView = new DarkUI.Controls.DarkListView();
            this.SuspendLayout();
            // 
            // consoleListView
            // 
            this.consoleListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.consoleListView.Location = new System.Drawing.Point(0, 25);
            this.consoleListView.Name = "consoleListView";
            this.consoleListView.Size = new System.Drawing.Size(647, 293);
            this.consoleListView.TabIndex = 0;
            this.consoleListView.Text = "Console List View";
            // 
            // DockConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.consoleListView);
            this.DefaultDockArea = DarkUI.Docking.DarkDockArea.Bottom;
            this.DockText = "Console";
            this.Icon = global::OpenNFSUI.Properties.Resources.Console;
            this.Name = "DockConsole";
            this.Size = new System.Drawing.Size(647, 318);
            this.Load += new System.EventHandler(this.DockConsole_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkListView consoleListView;
    }
}
