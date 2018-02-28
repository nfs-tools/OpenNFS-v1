using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenNFSUI
{
    public partial class GamePathControl : UserControl
    {
        private string GameTitle;
        private Image Icon;

        public GamePathControl(string gameTitle, Image icon)
        {
            GameTitle = gameTitle;
            Icon = icon;
            InitializeComponent();
        }

        private void GamePathControl_Load(object sender, EventArgs e)
        {
            gameTitleLabel.Text = GameTitle;
            mainImage.BackgroundImage = Icon;
            pathLabel.Text = "";
        }
    }
}
