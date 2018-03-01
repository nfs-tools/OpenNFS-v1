namespace OpenNFSUI
{
    partial class ChoiceButton
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
            this.divider = new System.Windows.Forms.PictureBox();
            this.btnIcon = new System.Windows.Forms.PictureBox();
            this.btnTitle = new System.Windows.Forms.Label();
            this.btnDesc = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.divider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // divider
            // 
            this.divider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.divider.BackgroundImage = global::OpenNFSUI.Properties.Resources.shadowdivider;
            this.divider.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.divider.Cursor = System.Windows.Forms.Cursors.Hand;
            this.divider.Location = new System.Drawing.Point(86, 13);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(21, 58);
            this.divider.TabIndex = 0;
            this.divider.TabStop = false;
            // 
            // btnIcon
            // 
            this.btnIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnIcon.BackgroundImage = global::OpenNFSUI.Properties.Resources.icons8_check_mark_symbol;
            this.btnIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIcon.Location = new System.Drawing.Point(8, 13);
            this.btnIcon.Name = "btnIcon";
            this.btnIcon.Size = new System.Drawing.Size(72, 58);
            this.btnIcon.TabIndex = 0;
            this.btnIcon.TabStop = false;
            // 
            // btnTitle
            // 
            this.btnTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTitle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTitle.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTitle.Location = new System.Drawing.Point(113, 13);
            this.btnTitle.Name = "btnTitle";
            this.btnTitle.Size = new System.Drawing.Size(378, 36);
            this.btnTitle.TabIndex = 1;
            this.btnTitle.Text = "Button Titleg";
            // 
            // btnDesc
            // 
            this.btnDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDesc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDesc.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDesc.Location = new System.Drawing.Point(114, 49);
            this.btnDesc.Name = "btnDesc";
            this.btnDesc.Size = new System.Drawing.Size(378, 27);
            this.btnDesc.TabIndex = 1;
            this.btnDesc.Text = "Button Description";
            // 
            // ChoiceButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.Controls.Add(this.btnDesc);
            this.Controls.Add(this.btnTitle);
            this.Controls.Add(this.btnIcon);
            this.Controls.Add(this.divider);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ForeColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ChoiceButton";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(499, 92);
            this.Load += new System.EventHandler(this.ChoiceButton_Load);
            ((System.ComponentModel.ISupportInitialize)(this.divider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox divider;
        private System.Windows.Forms.PictureBox btnIcon;
        private System.Windows.Forms.Label btnTitle;
        private System.Windows.Forms.Label btnDesc;
    }
}
