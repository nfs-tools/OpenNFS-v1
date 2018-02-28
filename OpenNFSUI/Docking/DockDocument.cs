using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkUI.Docking;
using DarkUI.Forms;

namespace OpenNFSUI.Docking
{
    public partial class DockDocument : DarkDocument
    {
        public DockDocument()
        {
            InitializeComponent();
            
            txtDocument.SelectionStart = txtDocument.Text.Length;
        }

        public DockDocument(string text, string content, Image icon) : this()
        {
            DockText = text;
            txtDocument.Text = content;
            Icon = icon;

            txtDocument.SelectionStart = txtDocument.Text.Length;
        }

        public override void Close()
        {
            var result = DarkMessageBox.ShowWarning(@"You will lose any unsaved changes. Continue?", @"Close document", DarkDialogButton.YesNo);
            if (result == DialogResult.No)
                return;

            base.Close();
        }
    }
}
