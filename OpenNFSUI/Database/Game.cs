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

        public Game(string title, string exeFileName, Image icon)
        {
            Title = title;
            ExectuableFileName = exeFileName;
            Icon = icon;
        }
    }
}
