using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUi.Messages
{
    /// <summary>
    /// Sent when a texture view needs to be shown.
    /// </summary>
    public class OpenTextureMessage
    {
        /// <summary>
        /// The group ID
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The texture hash.
        /// </summary>
        public uint Hash { get; set; }
    }
}
