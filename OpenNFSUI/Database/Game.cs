using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace OpenNFSUI.Database
{
    public class Game
    {
        public const int NFS_HP2_ID = 1000;
        public const int NFS_UG_ID = 1001;
        public const int NFS_UG2_ID = 1002;
        public const int NFS_MW_ID = 1003;
        public const int NFS_CARBON_ID = 1004;
        public const int NFS_PROSTREET_ID = 1005;
        public const int NFS_UNDERCOVER_ID = 1006;
        public const int NFS_WORLD_ID = 1007;

        /// <summary>
        /// The ID of this <see cref="Game"/>.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// <see cref="Game"/> title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// <see cref="Game"/> exe file name.
        /// <para>Example: speed.exe</para>
        /// </summary>
        public string ExectuableFileName { get; private set; }

        /// <summary>
        /// <see cref="Game"/>'s <see cref="Image"/> icon.
        /// </summary>
        public Image Icon { get; private set; }

        public Game(int id, string title, string exeFileName, Image icon)
        {
            ID = id;
            Title = title;
            ExectuableFileName = exeFileName;
            Icon = icon;
        }
    }
}
