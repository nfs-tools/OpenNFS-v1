using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibOpenNFS.DataModels;
using WpfUi.ViewModel.Data;

namespace WpfUi.Messages
{
    /// <summary>
    /// Sent when a texture pack view needs to be shown.
    /// </summary>
    public class OpenTexturePackMessage
    {
        /// <summary>
        /// The texture pack resource.
        /// </summary>
        public TexturePackResource Pack { get; set; }
    }
}
