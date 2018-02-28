using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkUI.Controls;
using DarkUI.Docking;
using OpenNFSUI.Explorer;

namespace OpenNFSUI.Docking
{
    public partial class DockExplorerBrowser : DarkToolWindow
    {
        public ListView FileListView { get; private set; }
        public ExplorerItem ExplorerItem { get; private set; }

        public DockExplorerBrowser()
        {
            InitializeComponent();

            listView1.Columns.Add("Name", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("Type", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("Size", 40, HorizontalAlignment.Left);
            listView1.Columns.Add("Full Path", 40, HorizontalAlignment.Right);
            //listView1.SmallImageList = iconsImageList;
            //listView1.LargeImageList = iconsImageList;
            
            listView1.View = View.Details;

            FileListView = listView1;
        }

        private void DockExplorerBrowser_Load(object sender, EventArgs e)
        {
            
        }
    }
}
