using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenNFSUI.Database;

namespace OpenNFSUI
{
    public static class Program
    {
        public static Config MainConfig { get; set; }

        public static bool FirstTimeUsage { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            MainConfig = Config.GetConfig();

            if(MainConfig == null)
            {
                MainConfig = new Config();
                

                MainConfig.SaveConfig();
            }

            FirstTimeUsage = MainConfig.FirstTimeUse;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new GreetForm());

        }
    }
}
