using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenNFSUI.Database;
using Newtonsoft.Json;

namespace OpenNFSUI
{
    public partial class GameDetectionForm : Form
    {
        public GameDetectionForm()
        {
            InitializeComponent();
        }

        private void GameDetectionForm_Load(object sender, EventArgs e)
        {
            Game[] supportedGames = SupportedDatabase.SupportedGames;

            int numberOfGamesOnLeft = SupportedDatabase.SupportedGames.Length - 3;
            int numberOfGamesOnRight = SupportedDatabase.SupportedGames.Length - numberOfGamesOnLeft;

            for (int i = numberOfGamesOnLeft; i-- > 0;)
            {
                GamePathControl gpc = new GamePathControl(supportedGames[i].Title, supportedGames[i].Icon);
                gpc.Dock = DockStyle.Top;
                leftPanel.Controls.Add(gpc);
            }
            
            for(int i = SupportedDatabase.SupportedGames.Length - 1; i >= numberOfGamesOnLeft; i--)
            {
                GamePathControl gpc = new GamePathControl(supportedGames[i].Title, supportedGames[i].Icon);
                gpc.Dock = DockStyle.Top;
                rightPanel.Controls.Add(gpc);
            }

            Config config = new Config();

        }
    }
}
