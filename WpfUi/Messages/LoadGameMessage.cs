using LibOpenNFS.Core;

namespace WpfUi.Messages
{
    /// <summary>
    /// The message that is sent to load a game directory.
    /// </summary>
    public class LoadGameMessage
    {
        /// <summary>
        /// The fully qualified directory path.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The <see cref="NFSGame"/> value that was determined for the directory.
        /// </summary>
        public NFSGame Game { get; set; }
    }
}
