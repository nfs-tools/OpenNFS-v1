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
        public GreetForm()
        {
            InitializeComponent();
        }

        private void GreetForm_Load(object sender, EventArgs e)
        {
            versionLabel.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
