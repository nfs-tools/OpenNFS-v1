using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFS.UI
{
    public interface IEntry
    {
        int Key { get; set; }
        string Name { get; set; }
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
        bool IsEnabled { get; set; }
        bool IsChecked { get; set; }
        IEntry Parent { get; set; }
    }
}
