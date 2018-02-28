namespace OpenNFSUI.Docking
{
    partial class DockDocument
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtDocument = new DarkUI.Controls.DarkTextBox();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // txtDocument
            // 
            this.txtDocument.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDocument.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDocument.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDocument.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDocument.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDocument.Location = new System.Drawing.Point(0, 0);
            this.txtDocument.Multiline = true;
            this.txtDocument.Name = "txtDocument";
            this.txtDocument.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtDocument.Size = new System.Drawing.Size(257, 237);
            this.txtDocument.TabIndex = 2;
            this.txtDocument.WordWrap = false;
            // 
            // DockDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtDocument);
            this.Name = "DockDocument";
            this.Size = new System.Drawing.Size(257, 237);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private DarkUI.Controls.DarkTextBox txtDocument;
    }
}
