using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNFS.UI
{
    public class CarDefinitionEntry : Entry
    {
        public string IDOne
        {
            get; set;
        }
        public string IDTwo
        {
            get; set;
        }
        public string ModelPath
        {
            get; set;
        }
        public string Maker
        {
            get; set;
        }

        public string FormattedName
        {
            get
            {
                return string.Format("{0} {1}", Maker, IDOne);
            }
        }
    }
}
