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
using DarkUI.Controls;

using static OpenNFSUI.Extensions.Methods;

namespace OpenNFSUI.Docking
{
    public partial class DockHexViewer : DarkDocument
    {
        public byte[] ByteArray { get; private set; }
        public string Title { get; private set; }

        public DockHexViewer(string title, byte[] byteArray)
        {
            Icon = Properties.Resources.document_16xLG;

            DockText = title;
            ByteArray = byteArray;

            InitializeComponent();

            try
            {
                AddHexBox();
            }
            catch { }
        }

        public void AddHexBox()
        {
            uint offset = 0x00000000;

            foreach (byte[] slice in ByteArray.Slices(16))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format(" {0}   ", offset.ToString("X8")));

                for(int i = 0; i < slice.Length; i++)
                {
                    string space = "";
                    if (i != slice.Length - 1)
                        space = " ";

                    sb.Append(string.Format("{0}{1}", slice[i].ToString("X2"), space));
                }

                hexTextBox.Text = string.Format("{0}{1}{2}", hexTextBox.Text, sb.ToString(), Environment.NewLine);

                offset = offset + 0x10;
            }

            hexTextBox.SelectionStart = hexTextBox.Text.Length;

        }

        private void hexTextBox_MouseDown(object sender, MouseEventArgs e)
        {
            DarkTextBox textBox = (DarkTextBox)sender;
            int index = textBox.GetCharIndexFromPosition(textBox.PointToClient(Cursor.Position));
            
            int posStart = textBox.SelectionStart;

            char delimiter = ' ';

            int start = textBox.SelectionStart;
            if (start < 1) start = 1;

            int left = textBox.Text.LastIndexOf(delimiter, start - 1);
            int right = textBox.Text.IndexOf(delimiter, start);
            if (right == -1) right = textBox.Text.Length;

            textBox.SelectionStart = left + 1;
            textBox.SelectionLength = right - left - 1;

        }
    }
}
