using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibOpenNFS.Core;

using static OpenNFSUI.Extensions.Methods;

namespace OpenNFSUI
{
    public partial class FirstTimeForm : Form
    {
        private static FirstTimeForm form;

        Dictionary<ChoiceButton, NFSGame> buttonsDictionary = new Dictionary<ChoiceButton, NFSGame>();

        ChoiceButton[] formButtons = {
            new ChoiceButton("Add a game directory", "Select a game directory that OpenNFS can work with", Properties.Resources.plus_512, (() =>
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if(fbd.ShowDialog() == DialogResult.OK)
                {
                    NFSGame nfsGame = GetNFSGameFromPath(fbd.SelectedPath);
                    string fullPath = fbd.SelectedPath;
                    switch(nfsGame)
                    {
                        case NFSGame.HotPursuit2:
                            Program.MainConfig.HP2DirPath = fullPath;
                            break;

                       case NFSGame.Underground:
                            Program.MainConfig.UGDirPath = fullPath;
                            break;

                        case NFSGame.Underground2:
                            Program.MainConfig.UG2DirPath = fullPath;
                            break;

                        case NFSGame.MW:
                            Program.MainConfig.MWDirPath = fullPath;
                            break;

                        case NFSGame.Carbon:
                            Program.MainConfig.CarbonDirPath = fullPath;
                            break;

                        case NFSGame.ProStreet:
                            Program.MainConfig.ProStreetDirPath = fullPath;
                            break;

                        case NFSGame.Undercover:
                            Program.MainConfig.UndercoverDirPath = fullPath;
                            break;

                        case NFSGame.World:
                            Program.MainConfig.WorldDirPath = fullPath;
                            break;

                        case NFSGame.Undetermined:
                            MessageBox.Show("Could not detect game.");
                            break;
                    }

                    if (nfsGame != NFSGame.Undetermined)
                    {
                        Program.MainConfig.SaveConfig();

                        Thread t = new Thread(new ThreadStart(ThreadProcRunTool));
                        t.Start();
                        form.Close();
                    }
                }
            })),
        };

        public FirstTimeForm()
        {
            InitializeComponent();
            form = this;
        }

        public void AddButton(ChoiceButton cb)
        {
            cb.Dock = DockStyle.Top;
            panelButtons.Controls.Add(cb);
        }

        private void FirstTimeForm_Load(object sender, EventArgs e)
        {
            for (int i = formButtons.Length; i-- > 0;)
            {
                AddButton(formButtons[i]);
            }
        }

        private static void ThreadProcRunTool()
        {
            Application.Run(new MainForm());
        }
    }
}
