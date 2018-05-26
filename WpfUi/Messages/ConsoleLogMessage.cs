using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfUi.ViewModel.Data;

namespace WpfUi.Messages
{
    /// <summary>
    /// The message that is sent to print a message to the console.
    /// </summary>
    public class ConsoleLogMessage
    {
        public MessageLevel Level { get; set; }

        public string Message { get; set; }
    }
}
