using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFS.UI
{
    public class TPKItemEntry : Entry
    {
        public new string Name
        {
            get;
        }

        public int Hash
        {
            get; set;
        }
    }

    public class TPKEntry : Group
    {
        public new IList<TPKEntry> SubGroups { get; set; }
        public new IList<TPKItemEntry> Entries { get; set; }
        public string Identifier
        {
            get; set;
        }
        public string Path
        {
            get; set;
        }
        public string FormattedName
        {
            get
            {
                return string.Format("{0} [{1}]", Path, Identifier);
            }
        }
    }
}
