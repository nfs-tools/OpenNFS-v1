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
using System.Reflection;

namespace OpenNFSUI
{
    /// <summary>
    /// The first form that appears when you open the tool.
    /// </summary>
    public partial class GreetForm : Form
    {
        /// <summary>
        /// This very form.
        /// </summary>
        private static GreetForm form;

        /// <summary>
        /// Form's <see cref="ChoiceButton"/> array.
        /// </summary>
        ChoiceButton[] formButtons = {
            new ChoiceButton("Start Modding", "Select a game and run the tool", Properties.Resources.opennfs_logo_shadowless, (() =>
            {
                Thread t = new Thread(new ThreadStart(ThreadProcRunTool));
                t.Start();
                form.Close();
            })),
            new ChoiceButton("Settings", "Change settings", Properties.Resources.spe_settings, (() => { })),
        };
        

        public GreetForm()
        {
            form = this;
            InitializeComponent();
        }

        private static void ThreadProcRunTool()
        {
            Application.Run(new MainForm());
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
