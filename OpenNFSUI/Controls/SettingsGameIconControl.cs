using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenNFSUI.Database;

using static OpenNFSUI.Extensions.Methods;

namespace OpenNFSUI.Controls
{
    
    public partial class SettingsGameIconControl : UserControl
    {
        public Game Game { get; private set; }
        public string DirectoryPath { get; private set; }
        public Image Icon { get; private set; }

        public PictureBox MainPB { get { return iconPB; } }

        public SettingsGameIconControl(Game game, string path, Image icon)
        {
            Game = game;
            DirectoryPath = path;
            Icon = icon;
            InitializeComponent();
        }

        private void SettingsGameIconControl_Load(object sender, EventArgs e)
        {
            iconPB.BackgroundImage = Icon;

            ApplyGameIconRendererGrayscaleByPathValidation(DirectoryPath, MainPB);

        }

        /// <summary>
        /// Ignore the fancy name for this function. If the path is null or invalid then grayscale the image.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private void ApplyGameIconRendererGrayscaleByPathValidation(string path, PictureBox pb)
        {
            bool isValid = IsPathValid(path);

            if (isValid == false)
                pb.BackgroundImage = ToolStripRenderer.CreateDisabledImage(pb.BackgroundImage);
        }
    }
}
