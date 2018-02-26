using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkUI.Controls;

namespace OpenNFSUI.Explorer
{
    public class ExplorerTreeNode : DarkTreeNode
    {
        private ExplorerItem explorerItem;

        public ExplorerItem ExplorerItem { get { return explorerItem; } set { explorerItem = value; } }
        public string Path { get; private set; }

        public ExplorerTreeNode(ExplorerItem explorerItem)
        {
            this.explorerItem = explorerItem;
            Text = explorerItem.Name;
            Path = explorerItem.FullPath;
        }
    }
}
