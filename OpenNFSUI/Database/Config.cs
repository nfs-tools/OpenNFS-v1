using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace OpenNFSUI.Database
{
    public class Config
    {
        public const string CONFIG_PATH = "config.json";

        // Games Directory Paths

        /// <summary>
        /// Need for Speed: Underground Directory Path
        /// </summary>
        public string UGDirPath { get; set; }

        /// <summary>
        /// Need for Speed: Underground 2 Directory Path
        /// </summary>
        public string UG2DirPath { get; set; }

        /// <summary>
        /// Need for Speed: Most Wanted Directory Path
        /// </summary>
        public string MWDirPath { get; set; }

        /// <summary>
        /// Need for Speed: Carbon Directory Path
        /// </summary>
        public string CarbonDirPath { get; set; }

        /// <summary>
        /// Need for Speed: ProStreet Directory Path
        /// </summary>
        public string ProStreetDirPath { get; set; }

        /// <summary>
        /// Need for Speed: Undercover Directory Path
        /// </summary>
        public string UndercoverDirPath { get; set; }

        /// <summary>
        /// Need for Speed: World Directory Path
        /// </summary>
        public string WorldDirPath { get; set; }

        /// <summary>
        /// Returns a value that indicates whether the program is being used for the first time or not.
        /// </summary>
        public bool FirstTimeUse { get; set; }

        public Config()
        {
            FirstTimeUse = true;
        }

        public static Config GetConfig()
        {
            try
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(CONFIG_PATH));
            }
            catch
            {
                return null;
            }
        }

        public bool SaveConfig()
        {
            FirstTimeUse = false;
            try
            {
                File.WriteAllText(CONFIG_PATH, JsonConvert.SerializeObject(this, Formatting.Indented));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
