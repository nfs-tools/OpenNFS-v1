using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace OpenNFSUI
{
    public partial class GreetForm : Form
    {
        ChoiceButton[] formButtons = {
            new ChoiceButton("Start Modding", "Select a game and run the tool", Properties.Resources.opennfs_logo_shadowless, (() => { })),
            new ChoiceButton("Settings", "Change settings and adjust preferences", Properties.Resources.spe_settings, (() => { })),
        };

        public GreetForm()
        {
            InitializeComponent();
        }

        private void GreetForm_Load(object sender, EventArgs e)
        {
            versionLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            for (int i = formButtons.Length; i-- > 0;)
            {
                formButtons[i].Dock = DockStyle.Top;
                btnPanelContainer.Controls.Add(formButtons[i]);
            }

        }
    }
}
