using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfUi.Messages
{
    /// <summary>
    /// The message that is sent to update the window title.
    /// </summary>
    public class WindowTitleMessage
    {
        /// <summary>
        /// The new window title.
        /// </summary>
        public string Title { get; set; }
    }
}
