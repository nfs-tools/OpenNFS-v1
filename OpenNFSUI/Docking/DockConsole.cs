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
using DarkUI.Controls;
using DarkUI.Forms;

namespace OpenNFSUI.Docking
{
    public partial class DockConsole : DarkToolWindow
    {
        public DockConsole()
        {
            InitializeComponent();
        }

        private void DockConsole_Load(object sender, EventArgs e)
        {
        }

        public void AddMessage(string text, Color color)
        {
            DarkListItem item = new DarkListItem(text);
            item.TextColor = color;
            consoleListView.Items.Add(item);
            consoleListView.Focus();
            consoleListView.SelectItem(consoleListView.Items.Count - 1);
            consoleListView.ScrollTo(new Point(0, GetItemsSizeY()));
        }

        private int GetItemsSizeY()
        {
            int y = 0;
            for(int i = 0; i < consoleListView.Items.Count; i++)
            {
                y = y + consoleListView.Size.Height;
            }

            return y;
        }
    }
}
