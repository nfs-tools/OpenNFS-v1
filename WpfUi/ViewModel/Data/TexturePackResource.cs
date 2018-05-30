using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOpenNFS.DataModels;

namespace WpfUi.ViewModel.Data
{
    /// <summary>
    /// Resource class for a <see cref="TexturePack"/>.
    /// </summary>
    public class TexturePackResource : GameResource
    {
        /// <summary>
        /// The group ID.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The pack hash.
        /// </summary>
        public uint Hash { get; set; }

        /// <summary>
        /// The pack name.
        /// </summary>
        public string PackName { get; set; }

        /// <summary>
        /// The tooltip text.
        /// </summary>
        public string ToolTip { get; set; }

        public TexturePackResource(uint hash)
        {
            Hash = hash;
        }
    }
}
