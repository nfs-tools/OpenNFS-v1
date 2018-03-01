namespace OpenNFSUI.Docking
{
    partial class DockHexViewer
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
            this.offsetTextBox = new DarkUI.Controls.DarkTextBox();
            this.hexTextBox = new DarkUI.Controls.DarkTextBox();
            this.SuspendLayout();
            // 
            // offsetTextBox
            // 
            this.offsetTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.offsetTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.offsetTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.offsetTextBox.Enabled = false;
            this.offsetTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.offsetTextBox.ForeColor = System.Drawing.Color.DodgerBlue;
            this.offsetTextBox.Location = new System.Drawing.Point(0, 0);
            this.offsetTextBox.Name = "offsetTextBox";
            this.offsetTextBox.ReadOnly = true;
            this.offsetTextBox.Size = new System.Drawing.Size(589, 15);
            this.offsetTextBox.TabIndex = 0;
            this.offsetTextBox.Text = " Offset(h)  00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0\r\nF";
            this.offsetTextBox.WordWrap = false;
            // 
            // hexTextBox
            // 
            this.hexTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.hexTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.hexTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hexTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexTextBox.ForeColor = System.Drawing.Color.White;
            this.hexTextBox.Location = new System.Drawing.Point(0, 15);
            this.hexTextBox.Multiline = true;
            this.hexTextBox.Name = "hexTextBox";
            this.hexTextBox.ReadOnly = true;
            this.hexTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.hexTextBox.Size = new System.Drawing.Size(589, 345);
            this.hexTextBox.TabIndex = 1;
            this.hexTextBox.WordWrap = false;
            this.hexTextBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.hexTextBox_MouseDown);
            // 
            // DockHexViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.Controls.Add(this.hexTextBox);
            this.Controls.Add(this.offsetTextBox);
            this.Name = "DockHexViewer";
            this.Size = new System.Drawing.Size(589, 360);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DarkUI.Controls.DarkTextBox offsetTextBox;
        private DarkUI.Controls.DarkTextBox hexTextBox;
    }
}
