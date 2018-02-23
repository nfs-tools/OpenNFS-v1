using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibOpenNFS.DataModels
{
    public class LanguagePack : BaseModel
    {
        public LanguagePack(long id, long size) : base(id, size)
        {
        }

        public uint NumStrings
        {
            get
            {
                return _numStrings;
            }
            set
            {
                _numStrings = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private uint _numStrings;
        private string _name;
    }
}
