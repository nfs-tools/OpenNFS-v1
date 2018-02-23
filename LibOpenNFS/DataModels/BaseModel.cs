using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibOpenNFS.DataModels
{
    public abstract class BaseModel
    {
        public BaseModel(long id, long size)
        {
            this.id = id;
            this.size = size;
        }

        public long GetId()
        {
            return id;
        }

        public long GetSize()
        {
            return size;
        }

        protected long id;
        protected long size;
    }
}
