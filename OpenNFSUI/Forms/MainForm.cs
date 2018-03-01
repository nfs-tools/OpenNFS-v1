using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DarkUI.Forms;
using DarkUI.Controls;
using DarkUI.Docking;
using OpenNFSUI.Explorer;
using OpenNFSUI.Docking;

using static OpenNFSUI.Extensions.Methods;

namespace OpenNFSUI
{
    public partial class MainForm : Form
    {
        List<ExplorerItem> explorerItems = new List<ExplorerItem>();
        List<DarkToolWindow> toolWindows = new List<DarkToolWindow>();

        // UI components
        DockConsole dockConsole = new DockConsole();
        DockExplorer dockExplorer = new DockExplorer();
        DockExplorerBrowser dockExplorerBrowser = new DockExplorerBrowser();
        DarkTreeView dockExplorerTreeView;
        DarkListView dockConsoleListView;

        public MainForm()
        {
            InitializeComponent();
        }

        #region Initializers
        private void InitializeDockPanel()
        {
            SetupToolWindow(dockConsole);
            SetupToolWindow(dockExplorer);
            SetupToolWindow(dockExplorerBrowser);

            Application.AddMessageFilter(mainDockPanel.DockContentDragFilter);
            Application.AddMessageFilter(mainDockPanel.DockResizeFilter);

            dockExplorerTreeView = (DarkTreeView)dockExplorer.Controls.Find("explorerTreeView", true)[0];
            dockConsoleListView = (DarkListView)dockConsole.Controls.Find("consoleListView", true)[0];
        }

        private void InitializeBrowserTreeView()
        {
            dockExplorerTreeView.Nodes.Clear();
            explorerItems.Clear();

            ListDirectory(dockExplorerTreeView, @"D:\Games\Electronic Arts\Need for Speed Carbon");
            dockExplorerTreeView.Nodes[0].Expanded = true;
        }

        private void InitializeToolStripDropdown()
        {
            for(int i = 0; i < toolWindows.Count; i++)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(toolWindows[i].DockText, toolWindows[i].Icon);
                item.Name = toolWindows[i].DockText;
                item.Checked = mainDockPanel.ContainsContent(toolWindows[i]);
                item.Click += ToolStripWindowItem_Click;
                twsmWindow.DropDownItems.Add(item);                
            }            
        }

        private void InitializeFileBrowser()
        {
            dockExplorerBrowser.FileListView.SmallImageList = iconsImageList;
            dockExplorerBrowser.FileListView.LargeImageList = iconsImageList;
            dockExplorerBrowser.FileListView.StateImageList = iconsImageList;
            ShowExplorerItemsInListView(explorerItems[0], dockExplorerBrowser.FileListView);
        }
        #endregion

        #region Functions
        private void ConsoleOutput(string text, Color color)
        {
            dockConsole.AddMessage(text, color);
        }

        private void CreateDocumentWindow(string title, string contents)
        {
            var newFile = new DockDocument(title, contents, Properties.Resources.document_16xLG);
            mainDockPanel.AddContent(newFile);

            //var newFile = new DockHexViewer("file.bin", File.ReadAllBytes(@"E:\My Projects\Github\OpenNFS\OpenNFSUI\Resources\RefactoringLog_12810.png"));
            //mainDockPanel.AddContent(newFile);
        }

        private void SetupToolWindow(DarkToolWindow toolWindow)
        {
            toolWindow.ParentChanged += ToolWindow_ParentChanged;
            toolWindows.Add(toolWindow);
            
            mainDockPanel.AddContent(toolWindow);
        }

        private void ListDirectory(DarkTreeView treeView, string path)
        {
            treeView.Nodes.Clear();

            var rootDirectoryInfo = new ExplorerItem(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private void ShowExplorerItemsInListView(ExplorerItem explorerItems, ListView listView)
        {
            for(int i = 0; i < explorerItems.Items.Count; i++)
            {
                int ImageIndex = 0;

                ExplorerItem explorerItem = explorerItems.Items[i];

                if (!explorerItem.IsFile)
                    ImageIndex = FileExtensionsData.FILE_FOLDER;
                else ImageIndex = explorerItem.FileData.ImageIndex;

                ListViewItem item = new ListViewItem(explorerItem.Name, ImageIndex);


                if(explorerItem.IsFile)
                    item.SubItems.Add(explorerItem.FileData.Type);
                else item.SubItems.Add("Directory");
                item.SubItems.Add(ParseFileSize(explorerItem.Size));
                item.SubItems.Add(explorerItem.FullPath);
                listView.Items.Add(item);
            }

            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
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

        private bool ToggleToolWindow(DarkToolWindow toolWindow)
        {
            bool isChecked = false;

            if (toolWindow.DockPanel == null)
            {
                mainDockPanel.AddContent(toolWindow);
                isChecked = true;
            }
            else
                mainDockPanel.RemoveContent(toolWindow);

            return isChecked;
        }
        #endregion

        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeDockPanel();
            InitializeBrowserTreeView();
            InitializeToolStripDropdown();
            InitializeFileBrowser();

            CreateDocumentWindow("title", "FUCK");

            ConsoleOutput("Ready", Color.White);
        }

        private void browserTreeView_Click(object sender, EventArgs e)
        {

        }

        private void ToolWindow_ParentChanged(object sender, EventArgs e)
        {
            DarkToolWindow item = (DarkToolWindow)sender;
            // item.Parent
            ToolStripItem[] stripItems = twsmWindow.DropDownItems.Find(item.DockText, true);
            if (stripItems.Length > 0)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)stripItems[0];
                menuItem.Checked = item.Parent != null;
            }
        }

        private void ToolStripWindowItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            item.Checked = ToggleToolWindow(toolWindows.Find(x => x.DockText == item.Text));
        }
        #endregion
    }
}
