using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkUI.Forms;
using DarkUI.Controls;
using OpenNFSUI.Explorer;

namespace OpenNFSUI
{
    public partial class MainForm : Form
    {
        List<ExplorerItem> explorerItems = new List<ExplorerItem>();

        public MainForm()
        {
            InitializeComponent();
        }

        #region Initializers
        private void InitializeBrowserTreeView()
        {
            browserTreeView.Nodes.Clear();
            explorerItems.Clear();

            ListDirectory(browserTreeView, AppDomain.CurrentDomain.BaseDirectory);
            browserTreeView.Nodes[0].Expanded = true;
            
        }
        #endregion

        #region Functions
        private void ListDirectory(DarkTreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new ExplorerItem(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private ExplorerTreeNode CreateDirectoryNode(ExplorerItem explorerItem)
        {
            var directoryNode = new ExplorerTreeNode(explorerItem);

            foreach (var item in explorerItem.Items)
            {
                if (item.IsFile)
                {
                    ExplorerTreeNode node = new ExplorerTreeNode(item);

                    directoryNode.Nodes.Add(node);

                }
                else
                {
                    explorerItems.Add(explorerItem);
                    directoryNode.Nodes.Add(CreateDirectoryNode(item));
                }
            }

            return directoryNode;
        }
        #endregion

        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeBrowserTreeView();
        }

        private void browserTreeView_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
