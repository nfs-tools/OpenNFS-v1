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
        /// The texture pack entity.
        /// </summary>
        public TexturePack Pack { get; set; }

        /// <summary>
        /// The tooltip text.
        /// </summary>
        public string ToolTip => $"{Pack.Path} / Textures: {Pack.Textures.Count}";

        public TexturePackResource(TexturePack pack)
        {
            Pack = pack;
        }
    }
}
