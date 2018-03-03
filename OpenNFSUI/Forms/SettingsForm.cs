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
using DarkUI.Docking;
using DarkUI.Controls;
using DarkUI.Forms;
using OpenNFSUI.Controls;
using OpenNFSUI.Database;

using static OpenNFSUI.Extensions.Methods;

namespace OpenNFSUI
{
    public partial class SettingsForm : Form
    {
        bool unsavedSettings = false;

        List<Panel> panelList;

        public SettingsForm()
        {
            InitializeComponent();
        }

        #region Functions
        #endregion

        #region Events

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            checkUpdatesCB.Checked = Program.MainConfig.CheckForUpdates;

            // Load panels into an array
            Control.ControlCollection arrayPanelCollection = arrayPanel.Controls;
            panelList = new List<Panel>();
            //panelArray.OrderByDescending(x => x.Name);
            panelList.AddRange(arrayPanelCollection.OfType<Panel>());
            panelList.Sort((x, y) => string.Compare(x.Name, y.Name));
            for (int i = 0; i < panelList.Count; i++)
            {
                string path = GetPathByGame(SupportedDatabase.SupportedGames[i]);

                Game game = SupportedDatabase.SupportedGames[i];
                SettingsGameIconControl sgic = new SettingsGameIconControl(game, path, game.Icon);
                sgic.MainPB.MouseEnter += GameIcon_MouseEnter;
                panelList[i].Controls.Add(sgic);
            }

            // TODO: Minimize it to one function that takes care of all the paths and icons.
            /*ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.HP2DirPath, hp2PB);
            ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.UGDirPath, ugPB);
            ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.UG2DirPath, ug2PB);
            ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.MWDirPath, mwPB);
            ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.CarbonDirPath, carbonPB);
            ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.ProStreetDirPath, psPB);
            ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.UndercoverDirPath, ucPB);
            ApplyGameIconRendererGrayscaleByPathValidation(Program.MainConfig.WorldDirPath, worldPB);*/
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            if (unsavedSettings)
            {
                DialogResult unsavedDialog = DarkMessageBox.ShowWarning("You have unsaved changes, are you sure you want to close this window?", "Unsaved changes", DarkDialogButton.YesNo);
                if(unsavedDialog == DialogResult.Yes)
                {
                    Close();
                }
            }
            else Close();
        }

        private void checkUpdatesCB_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = ((DarkCheckBox)sender).Checked;

            if (isChecked)
                startupUpdateStatusLabel.Text = "The application will check for updates on startup.";
            else startupUpdateStatusLabel.Text = "The application will not check for updates on startup.";
        }

        private void appPanel_MouseEnter(object sender, EventArgs e)
        {
            gameDirStatusLabel.Text = "Click on a game to select it's directory.";
        }

        private void GameIcon_MouseEnter(object sender, EventArgs e)
        {
            string setString = "not set.";

            Control c = (Control)sender;
            SettingsGameIconControl main = (SettingsGameIconControl)c.Parent;


            if (IsPathValid(main.DirectoryPath))
                setString = main.DirectoryPath;

           gameDirStatusLabel.Text = string.Format("{0} directory path is {1}", main.Game.Title, setString);
        }
        #endregion
    }
}
